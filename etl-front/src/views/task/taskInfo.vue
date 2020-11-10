<template>
    <div style="margin:20px">
        <!-- 任务名称 -->
        <v-row>
            <v-col cols="2">任务名称</v-col>
            <v-col>
                <v-text-field v-model="dTask.NAME" dense outlined></v-text-field>
            </v-col>
        </v-row>
        <!-- 选择模型 -->
        <v-row>
            <v-col cols="2">选择模型</v-col>
            <v-col>
                <v-select
                    dense
                    outlined
                    v-model="dTask.MODEL_ID"
                    :items="ModelList"
                    item-text="NAME"
                    item-value="ID"
                ></v-select>
            </v-col>
        </v-row>
        <!-- 任务状态 -->
        <v-row>
            <v-col cols="2">任务状态</v-col>
            <v-col>
                <v-select dense outlined v-model="dTask.STATE" :items="dStateList"></v-select>
            </v-col>
        </v-row>
        <!-- 执行频率 -->
        <v-row>
            <v-col cols="2">执行频率</v-col>
            <v-col>
                <v-select dense outlined v-model="dTask.REPEAT_MODE" :items="dRepeatModeList"></v-select>
            </v-col>
        </v-row>
        <!-- 执行时间 -->
        <v-row v-if="dTask.REPEAT_MODE=='1'">
            <v-col cols="2">执行时间</v-col>
            <v-col>
                <v-text-field v-model="dTask.TIME_REGULAR" dense outlined></v-text-field>
            </v-col>
        </v-row>
        <!-- 时间间隔 -->
        <v-row v-if="dTask.REPEAT_MODE=='2'">
            <v-col cols="2">时间间隔（分钟）</v-col>
            <v-col>
                <v-text-field v-model="dTask.TIME_INTERVAL" dense outlined></v-text-field>
            </v-col>
        </v-row>
        <!-- 动态参数 -->
        <v-row v-for="param in dTask.PARAMS" :key="param.ID">
            <v-col>
                <span v-if="param.REQUIRED" style="color:#f00;">*</span>
                {{ '步骤：' + param.STEP_NAME + '，参数名：' + param.NODE_PARAM_ALIAS}}
            </v-col>
            <v-col>
                <v-text-field v-model="param.PARAM_VALUE" dense outlined></v-text-field>
            </v-col>
        </v-row>
        <!-- 保存、取消 -->
        <v-row>
            <v-col>
                <v-btn class="float-right mr-5" color="error" @click="Cancel">取消</v-btn>
                <v-btn class="float-right mr-5" color="primary" @click="Save">保存</v-btn>
            </v-col>
        </v-row>
    </div>
</template>

<script>
import { mapState, mapGetters, mapActions } from "vuex";

const dStateList = [
    { text: "正常", value: "1" },
    { text: "停用", value: "0" },
];
const dRepeatModeList = [
    { text: "按执行时间", value: "1" },
    { text: "按时间间隔", value: "2" },
    { text: "只执行一次", value: "3" },
];

export default {
    data() {
        return {
            dTask: {}, // 任务信息
            dStateList,
            dRepeatModeList,
        };
    },
    props: {
        // 任务id
        id: {
            type: String,
            default: "",
        },
    },
    computed: {
        ...mapState("TaskModule", ["TaskList"]),
        ...mapState("ModelModule", ["ModelList"]),
        ...mapGetters("API", ["urlGetDynamicParams", "urlSaveTask"]),
    },
    asyncComputed: {
        cModelParamList() {
            let url = `${this.urlGetDynamicParams}?model_id=${this.dTask.MODEL_ID}`;
            return this.$axios.get(url).then((response) => response.data);
        },
    },
    watch: {
        cModelParamList(list) {
            this.dTask.PARAMS = list.map((mParam) => {
                let tParam = this.dTask.PARAMS.find(
                    (tParam) => tParam.STEP_PARAM_ID === mParam.STEP_PARAM_ID
                );
                if (tParam) {
                    tParam = { ...tParam }; // 创建副本以供编辑
                } else {
                    tParam = {
                        ID: window.guid(),
                        STEP_PARAM_ID: mParam.STEP_PARAM_ID,
                        TASK_ID: this.dTask.ID,
                        PARAM_VALUE: "",
                    };
                }
                tParam.STEP_NAME = mParam.STEP_NAME;
                tParam.NODE_PARAM_ALIAS = mParam.NODE_PARAM_ALIAS;
                return tParam;
            });
        },
    },
    created() {
        let task = this.TaskList.find((task) => task.ID === this.id);
        if (task) {
            task = { ...task };
        } else {
            task = {
                ID: window.guid(),
                MODEL_ID: "",
                NAME: "",
                STATE: "1",
                REPEAT_MODE: "1",
                TIME_INTERVAL: "",
                TIME_REGULAR: "",
                PARAMS: [],
            };
        }
        this.dTask = task;
    },
    methods: {
        ...mapActions("TaskModule", ["RefreshTaskList"]),
        async Save() {
            let data = {
                taskJSON: JSON.stringify(this.dTask),
            };
            this.$axios
                .post(this.urlSaveTask, data)
                .then(() => {
                    this.$VMessage.success("保存成功");
                    this.RefreshTaskList();
                    this.$router.push("/task");
                })
                .catch((error) => {
                    this.$VMessage.error(error);
                });
        },
        Cancel() {
            this.$router.push("/task");
        },
    },
};
</script>