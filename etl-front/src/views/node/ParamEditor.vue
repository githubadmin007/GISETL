<template>
        <v-sheet style="margin:20px">
        <v-row>
            <v-col cols="4">参数名</v-col>
            <v-col>
                <v-text-field v-model="dParam.NAME" dense outlined></v-text-field>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="4">别名</v-col>
            <v-col>
                <v-text-field v-model="dParam.ALIAS" dense outlined></v-text-field>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="4">是否必填</v-col>
            <v-col>
                <v-combobox v-model="dParam.REQUIRED" :items="RequireItems" dense outlined></v-combobox>
            </v-col>
        </v-row>
        <!-- 保存、取消 -->
        <v-row>
            <v-col>
                <v-btn class="float-right mr-5" color="error" @click="Cancel">取消</v-btn>
                <v-btn class="float-right mr-5" color="primary" @click="Save">保存</v-btn>
            </v-col>
        </v-row>
    </v-sheet>
</template>

<script>
const RequireItems = ['是', '否'];

export default {
    data () {
        return {
            dParam: {},
            RequireItems
        }
    },
    props: {
        // 节点
        node: {
            type: Object,
            required: true
        },
        // 节点参数对象
        param: {
            type: Object,
            default: null
        },
    },
    methods: {
        // 保存
        Save () {
            if (this.param) {
                let index = this.node.PARAMS.indexOf(this.param);
                this.node.PARAMS.splice(index, 1, this.dParam);
            } else {
                this.node.PARAMS.push(this.dParam);
            }
            this.$emit("closewindow");
        },
        // 取消
        Cancel () {
            this.$emit("closewindow");
        }
    },
    created() {
        this.dParam = this.param ? 
        { ...this.param } :
        {
            ID: window.guid(),
            NODE_ID: this.node.ID,
            NAME: '',
            ALIAS: '',
            REQUIRED: ''
        };
    },
}
</script>
