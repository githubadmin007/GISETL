<template>
    <v-sheet class="ma-5">
        <v-row>
            <v-col cols="4">节点名称</v-col>
            <v-col>
                <v-text-field v-model="dNode.NAME" dense outlined></v-text-field>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="4">节点类名</v-col>
            <v-col>
                <v-text-field v-model="dNode.CLASS_NAME" dense outlined></v-text-field>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="4">输出类型</v-col>
            <v-col>
                <v-combobox v-model="dNode.OUTPUT_TYPE" :items="TypeItems" dense outlined></v-combobox>
            </v-col>
        </v-row>
        <!-- 参数、输入 -->
        <v-row>
            <!-- 参数 -->
            <v-col>
                <v-data-table 
                    hide-default-footer
                    class="elevation-1"
                    :headers="ParamHeader"
                    :items="dNode.PARAMS">
                    <template v-slot:[`header.actions`]>
                        <v-btn icon @click="AdddParam">
                            <v-icon>mdi-plus</v-icon>
                        </v-btn>
                    </template>
                    <template v-slot:[`item.actions`]="{ item }">
                        <v-btn icon @click="EditParam(item)">
                            <v-icon small>mdi-pencil</v-icon>
                        </v-btn>
                        <v-btn icon @click="DeleteParam(item)">
                            <v-icon small>mdi-delete</v-icon>
                        </v-btn>
                    </template>
                </v-data-table>
            </v-col>
            <!-- 输入 -->
            <v-col>
                <v-data-table 
                    hide-default-footer
                    class="elevation-1"
                    :headers="InputHeader"
                    :items="dNode.INPUTS">
                    <template v-slot:[`header.actions`]>
                        <v-btn icon @click="AdddInput">
                            <v-icon>mdi-plus</v-icon>
                        </v-btn>
                    </template>
                    <template v-slot:[`item.actions`]="{ item }">
                        <v-btn icon @click="EditInput(item)">
                            <v-icon small>mdi-pencil</v-icon>
                        </v-btn>
                        <v-btn icon @click="DeleteInput(item)">
                            <v-icon small>mdi-delete</v-icon>
                        </v-btn>
                    </template>
                </v-data-table>
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
import ParamEditor from './ParamEditor'
import InputEditor from './InputEditor'
import { mapState, mapGetters, mapActions } from 'vuex';

// 参数表头
const ParamHeader = [
    { text: "名称", value: "NAME" },
    { text: "别名", value: "ALIAS" },
    { text: "是否必填", value: "REQUIRED" },
    { text: "修改", value: "actions", sortable: false }
];
// 输入表头
const InputHeader = [
    { text: "名称", value: "NAME" },
    { text: "别名", value: "ALIAS" },
    { text: "类型", value: "TYPE" },
    { text: "修改", value: "actions", sortable: false }
];

export default {
    data() {
        return {
            dNode: {},
            ParamHeader,
            InputHeader
        };
    },
    props: {
        id: {
            type: String,
            default: "",
        },
    },
    computed: {
        ...mapState('NodeModule', ['NodeList', 'TypeItems']),
        ...mapGetters('API', ['urlSaveNode', 'urlGetNodeList']),
    },
    methods: {
        ...mapActions('NodeModule', ['RefreshNodeList']),
        // 新增参数
        AdddParam () {
            this.$VWindow({
                id: "window1",
                title: "新增参数",
                moveAble: true,
                component: ParamEditor,
                componentProps: {
                    node: this.dNode,
                },
            });
        },
        // 编辑参数
        EditParam (param) {
            this.$VWindow({
                id: "window1",
                title: "编辑参数",
                moveAble: true,
                component: ParamEditor,
                componentProps: {
                    node: this.dNode,
                    param
                },
            });
        },
        // 删除参数
        DeleteParam (param) {
            const index = this.dNode.PARAMS.indexOf(param);
            confirm(`是否要删除参数:“${param.NAME}”`) && this.dNode.PARAMS.splice(index, 1);
        },
        // 新增输入
        AdddInput () {
            this.$VWindow({
                id: "window1",
                title: "新增输入",
                moveAble: true,
                component: InputEditor,
                componentProps: {
                    node: this.dNode,
                },
            });
        },
        // 编辑输入
        EditInput (input) {
            this.$VWindow({
                id: "window1",
                title: "编辑输入",
                moveAble: true,
                component: InputEditor,
                componentProps: {
                    node: this.dNode,
                    input
                },
            });
        },
        // 删除参数
        DeleteInput (input) {
            const index = this.dNode.INPUTS.indexOf(input);
            confirm(`是否要删除输入:“${input.NAME}”`) && this.dNode.INPUTS.splice(index, 1);
        },
        // 保存
        async Save () {
            if (this.CheckRequired(this.dNode)) {
                let data = {
                    nodeJSON: JSON.stringify(this.dNode)
                };
                console.log(data);
                await this.$axios.post(this.urlSaveNode, data);
                this.$VMessage.success("保存成功");
                await this.RefreshNodeList();
                this.$router.push("/node");
            }
        },
        // 取消
        Cancel () {
            this.$router.push("/node");
        },
        // 检查必填项
        CheckRequired (node) {
            if (!node.NAME) {
                this.$VMessage.error('【节点名称】不能为空');
                return false;
            }
            if (!node.CLASS_NAME) {
                this.$VMessage.error('【节点类名】不能为空');
                return false;
            }
            if (!node.OUTPUT_TYPE) {
                this.$VMessage.error('【输出类型】不能为空');
                return false;
            }
            return true;
        }
    },
    created() {
        let node = this.NodeList.find((node) => node.ID === this.id);
        if (node) {
            node = { ...node };
        } else {
            node = {
                ID: window.guid(),
                NAME: "",
                CLASS_NAME: "",
                OUTPUT_TYPE: "",
                PARAMS: [],
                INPUTS: [],
            };
        }
        this.dNode = node;
    },
};
</script>
