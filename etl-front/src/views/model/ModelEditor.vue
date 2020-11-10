<template>
    <div style="height:100%;">
        <!-- 顶部工具条 -->
        <v-toolbar dense flat>
            <v-toolbar-title>
                <v-icon left>mdi-pencil</v-icon>模型编辑
            </v-toolbar-title>
            <v-spacer></v-spacer>
            <v-text-field
                v-model="dModel.NAME"
                label="模型名称"
                color="secondary"
                hide-details
                style="max-width: 300px"
            ></v-text-field>
            <v-btn class="ma-2" outlined @click="ConnectStep">
                <i class="iconfont icon-lianxian_icon"></i>
                节点连线
            </v-btn>
            <v-btn class="ma-2" outlined @click="SaveModel">
                <i class="iconfont icon-baocun"></i>
                保存模型
            </v-btn>
            <v-btn class="ma-2" color="error" @click="Back">
                <i class="iconfont icon-fanhui"></i>
                返回
            </v-btn>
        </v-toolbar>
        <!-- 工具栏 -->
        <v-card
            style="
                display: inline-block;
                height: 100%;
                width: 100px;
                margin-right: 10px;
                vertical-align: top;
            "
            color="#F5F5F5"
        >
            <v-btn
                v-for="type in NodeTypeList"
                :key="type.name"
                @click="NewStep(type)"
            >
                {{ type.cname }}
            </v-btn>
        </v-card>
        <!-- 模型画板 -->
        <v-card
            id="draw"
            style="
                display: inline-block;
                height: 100%;
                width: calc(100% - 115px);
            "
            color="#F5F5F5"
        >
            <svg width="100%" height="100%">
                <stepRelation
                    v-for="(relation, index) in cRelationList"
                    :key="index"
                    :relation="relation"
                />
                <stepNode
                    v-for="step in dStepList"
                    v-contextmenu="{
                        menuId: 'step-menu',
                        Data: step,
                        items: dStepMenuItems,
                    }"
                    :key="step.ID"
                    :step="step"
                    @click="OnClick($event, step)"
                    @mousedown="OnMouseDown($event, step)"
                />
            </svg>
        </v-card>
    </div>
</template>

<script>
import stepNode from "./stepNode";
import stepRelation from "./stepRelation";
import SelectStepInput from "./SelectStepInput";
import { mapState, mapGetters, mapActions } from "vuex";

const NodeTypeList = [
    {
        name: "stepInfo",
        cname: "普通节点",
        component: () => import("./stepInfo"),
    },
    {
        name: "FieldMappings",
        cname: "字段映射",
        component: () => import("./components/FieldMappings"),
    },
];

export default {
    components: {
        stepNode,
        stepRelation,
    },
    data() {
        return {
            NodeTypeList,
            dModel: {},
            dStepList: [], // 步骤节点
            dRelationList: [], // 步骤连接关系
            dMouseMode: "", // 鼠标模式（connect）
            dFrontStep: null, // 前一步骤
            dAfterStep: null, // 后一步骤
            dStepMenuItems: [
                {
                    text: "编辑",
                    click: this.EditStep,
                },
                {
                    text: "删除",
                    click: this.DeleteStep,
                },
            ],
        };
    },
    props: {
        // 模型id
        id: {
            type: String,
            default: "",
        },
    },
    computed: {
        ...mapState("ModelModule", ["ModelList"]),
        ...mapState("NodeModule", ["NodeList"]),
        ...mapGetters("API", [
            "urlGetStepList",
            "urlGetRelationList",
            "urlSaveModel",
        ]),
        cRelationList() {
            let _this = this;
            return this.dRelationList.map((r) => {
                return {
                    frontStep: _this.dStepList.find(
                        (step) => step.ID === r.FRONT_STEP_ID
                    ),
                    afterStep: _this.dStepList.find(
                        (step) => step.ID === r.AFTER_STEP_ID
                    ),
                    NODE_INPUT_ID: r.NODE_INPUT_ID,
                };
            });
        },
    },
    methods: {
        ...mapActions("ModelModule", ["RefreshModelList"]),
        // 获取步骤列表
        async GetStepList() {
            let url = `${this.urlGetStepList}?model_id=${this.dModel.ID}`;
            let response = await this.$axios.get(url);
            this.dStepList = response.data.map((step) => {
                step.WIDTH = step.WIDTH || 100;
                step.HEIGHT = step.HEIGHT || 50;
                return step;
            });
        },
        // 获取步骤关联关系列表
        async GetRelationList() {
            let url = `${this.urlGetRelationList}?model_id=${this.dModel.ID}`;
            let response = await this.$axios.get(url);
            this.dRelationList = response.data;
        },
        // 新增节点
        NewStep(type) {
            let node = this.NodeList.find(
                (node) => node.CLASS_NAME === type.name
            );
            let step = {
                ID: window.guid(),
                NODE_ID: node ? node.ID : "",
                MODEL_ID: this.dModel.ID,
                NAME: "",
                X: 0,
                Y: 0,
                WIDTH: 100,
                HEIGHT: 50,
                PARAMS: [],
            };
            this.$VWindow({
                id: "window1",
                title: "创建步骤",
                moveAble: true,
                component: type.component,
                componentProps: {
                    step,
                },
                beforeClose: (close) => {
                    if (step.NAME && step.NODE_ID) {
                        this.dStepList.push(step);
                    }
                    close();
                },
            });
        },
        // 编辑步骤
        EditStep(e, step) {
            let node = this.NodeList.find((node) => node.ID === step.NODE_ID);
            let NodeType = this.NodeTypeList.find(
                (type) => type.name === node.CLASS_NAME
            );
            this.$VWindow({
                id: "window1",
                title: "编辑步骤",
                moveAble: true,
                component: NodeType
                    ? NodeType.component
                    : () => import("./stepInfo"),
                componentProps: {
                    step,
                },
            });
        },
        // 删除步骤
        DeleteStep(e, step) {
            if (confirm(`是否删除步骤【${step.NAME}】`)) {
                // 移除连线后，连线后续节点的输入被释放，USED应设为false
                this.cRelationList.forEach((relation) => {
                    if (relation.frontStep == step) {
                        let usedInput = relation.afterStep.INPUTS.find(
                            (input) => input.ID == relation.NODE_INPUT_ID
                        );
                        if (usedInput) {
                            usedInput.USED = false;
                        }
                    }
                });
                // 移除连线
                this.dRelationList = this.dRelationList.filter((relation) => {
                    return (
                        relation.FRONT_STEP_ID != step.ID &&
                        relation.AFTER_STEP_ID != step.ID
                    );
                });
                // 移除节点
                this.dStepList.filterAndRemove((item) => item.ID === step.ID);
            }
        },
        // 连接步骤
        ConnectStep() {
            this.dMouseMode = "connect";
        },
        // 选择输入目标,（无匹配目标返回空；只有一个直接选中；有多个则弹窗选择）
        SelectInputTarget(frontStep, afterStep, callback) {
            let OUTPUT_TYPE = frontStep.OUTPUT_TYPE;
            let INPUTS = afterStep.INPUTS.filter((input) => {
                return !input.USED && input.TYPE === OUTPUT_TYPE;
            });
            if (INPUTS.length == 0) {
                callback(null);
            } else if (INPUTS.length == 1) {
                callback(INPUTS[0]);
            } else {
                this.$VWindow({
                    id: "window1",
                    title: "选择输入",
                    moveAble: true,
                    component: SelectStepInput,
                    componentProps: {
                        inputs: INPUTS,
                        callback: (input) => {
                            callback(input);
                        },
                    },
                });
            }
        },
        // 连线完成
        ConnectFinished() {
            this.SelectInputTarget(
                this.dFrontStep,
                this.dAfterStep,
                (input) => {
                    if (input) {
                        input.USED = true;
                        this.dRelationList.push({
                            ID: window.guid(),
                            MODEL_ID: this.dModel.ID,
                            FRONT_STEP_ID: this.dFrontStep.ID,
                            AFTER_STEP_ID: this.dAfterStep.ID,
                            NODE_INPUT_ID: input.ID,
                        });
                    } else {
                        this.$VMessage.error("目标节点没有合适的输入可供选择");
                    }
                    this.dFrontStep = null;
                    this.dAfterStep = null;
                    this.dMouseMode = "";
                }
            );
        },
        // 鼠标点击节点事件
        OnClick(e, step) {
            if (this.dMouseMode === "connect") {
                // 起始步骤不为空
                if (this.dFrontStep) {
                    if (this.dFrontStep === step) {
                        this.dFrontStep = null;
                    } else {
                        this.dAfterStep = step;
                        this.ConnectFinished();
                    }
                } else {
                    this.dFrontStep = step;
                }
            }
        },
        // 鼠标按下节点事件
        OnMouseDown(e, step) {
            let clientX = e.clientX;
            let clientY = e.clientY;
            var stepwidth = step.WIDTH;
            var stepheight = step.HEIGHT;
            var cardwidth = document
                .getElementById("draw")
                .getBoundingClientRect().width;
            var cardheight = document
                .getElementById("draw")
                .getBoundingClientRect().height;
            var maxwidth = cardwidth - stepwidth;
            var maxheight = cardheight - stepheight;
            let onmousemove = document.onmousemove;
            let onmouseup = document.onmouseup;
            document.onmousemove = (e) => {
                step.X += e.clientX - clientX;
                step.Y += e.clientY - clientY;
                clientX = e.clientX;
                clientY = e.clientY;
                //限定节点框移动范围
                step.X = step.X < 0 ? 0 : step.X;
                step.X = step.X > maxwidth ? maxwidth : step.X;
                step.Y = step.Y < 0 ? 0 : step.Y;
                step.Y = step.Y > maxheight ? maxheight : step.Y;
            };
            document.onmouseup = () => {
                document.onmousemove = onmousemove;
                document.onmouseup = onmouseup;
            };
        },
        // 保存模型
        async SaveModel() {
            if (!this.dModel.NAME) {
                this.$VMessage.error("请填写模型名称");
                return;
            }
            let data = {
                modelJSON: JSON.stringify(this.dModel),
                stepsJSON: JSON.stringify(this.dStepList),
                relationsJSON: JSON.stringify(this.dRelationList),
            };
            await this.$axios.post(this.urlSaveModel, data);
            this.$VMessage.success("保存成功");
            await this.RefreshModelList();
            this.$router.push("/model");
        },
        // 返回
        Back() {
            if (confirm(`即将离开此页面，请确认修改已保存`)) {
                this.$router.push("/model");
            }
        },
    },
    created() {
        let model = this.ModelList.find((model) => model.ID === this.id);
        if (model) {
            model = { ...model };
        } else {
            model = {
                ID: window.guid(),
                NAME: "",
                TIME: new Date().Format("yyyy/MM/dd HH:mm:ss"),
            };
        }
        this.dModel = model;
        if (this.id) {
            this.GetStepList();
            this.GetRelationList();
        }
    },
};
</script>