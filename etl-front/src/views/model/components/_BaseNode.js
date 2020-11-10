import { mapState } from "vuex";

export default {
    data() {
        return {
            dName: null,
            dNodeId: "",
        };
    },
    props: {
        // 步骤信息
        step: {
            type: Object,
            default: null,
        },
    },
    computed: {
        ...mapState("NodeModule", ["NodeList"]),
        // 步骤名
        cName: {
            get() {
                return this.dName === null ? this.step.NAME : this.dName;
            },
            set(value) {
                this.dName = value;
            },
        },
        // 当前选中节点id
        cNodeId: {
            get() {
                return this.dNodeId || this.step.NODE_ID;
            },
            set(value) {
                this.dNodeId = value;
            },
        },
        // 当前选中节点
        cNode() {
            return this.NodeList.find((node) => node.ID == this.cNodeId);
        },
        // 当前选中节点参数列表
        cNodeParamList() {
            let node = this.cNode;
            return node ? node.PARAMS : [];
        },
        // 步骤参数列表
        cStepParamList() {
            return this.cNodeParamList.map((nParam) => {
                let sParam = this.step.PARAMS.find(
                    (sParam) => sParam.NODE_PARAM_ID === nParam.ID
                );
                if (sParam) {
                    sParam = { ...sParam }; // 创建副本以供编辑
                } else {
                    sParam = {
                        ID: window.guid(),
                        NODE_PARAM_ID: nParam.ID,
                        STEP_ID: this.step.ID,
                        PARAM_VALUE: "",
                        IS_DYNAMIC: "否",
                    };
                }
                sParam.NAME = nParam.NAME;
                sParam.ALIAS = nParam.ALIAS;
                sParam.REQUIRED = nParam.REQUIRED;
                return sParam;
            });
        },
    },
    methods: {
        // 检查是否可以保存
        Check() {
            if (!this.cName) {
                this.$VMessage.error("请填写步骤名称");
                return false;
            }
            if (!this.cNodeId) {
                this.$VMessage.error("请选择步骤操作");
                return false;
            }
            for (let i in this.cStepParamList) {
                let param = this.cStepParamList[i];
                if (
                    param.IS_DYNAMIC === "否" &&
                    param.REQUIRED === "是" &&
                    param.PARAM_VALUE === ""
                ) {
                    this.$VMessage.error(`参数【${param.ALIAS}】为必填项`);
                    return false;
                }
            }
            return true;
        },
        // 保存
        SaveStep() {
            if (!this.Check()) return;
            this.step.NAME = this.cName;
            this.step.NODE_ID = this.cNodeId;
            this.step.PARAMS = this.cStepParamList;
            this.step.OUTPUT_TYPE = this.cNode.OUTPUT_TYPE;
            this.step.INPUTS = this.cNode.INPUTS.map((input) => {
                return {
                    ID: input.ID,
                    NAME: input.NAME,
                    TYPE: input.TYPE,
                    USED: false,
                };
            });
            this.$emit("closewindow");
        },
        // 取消
        Cancel() {
            this.$emit("closewindow");
        },
    },
};
