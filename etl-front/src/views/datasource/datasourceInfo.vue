<template>
    <div style="margin:20px">
        <v-row>
            <v-col cols="4">数据源名称</v-col>
            <v-col>
                <v-text-field v-model="cNAME" dense outlined></v-text-field>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="4">数据源类型</v-col>
            <v-col>
                <v-select dense outlined v-model="cTYPE" :items="TypeList"></v-select>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="4">文件路径</v-col>
            <v-col>
                <v-text-field v-model="cFILEPATH" dense outlined></v-text-field>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="4">连接信息</v-col>
            <v-col>
                <v-text-field v-model="cCONNECT_STR" dense outlined></v-text-field>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="4">服务地址</v-col>
            <v-col>
                <v-text-field v-model="cSERVER_URL" dense outlined></v-text-field>
            </v-col>
        </v-row>
        <v-row>
            <v-col>
                <v-btn color="primary" @click="Save">保存</v-btn>
                <v-btn color="error" @click="Cancel" style="margin:5px">取消</v-btn>
            </v-col>
        </v-row>
    </div>
</template>

<script>
import { mapState, mapGetters, mapActions } from "vuex";
export default {
    data() {
        return {
            TypeList: ["GDB", "SDE", "MDB", "SHP", "ORACLE", "MapServer"],
            cTYPE: "",
            cFILEPATH: "",
            cCONNECT_STR: "",
            cSERVER_URL: "",
            cID: "",
            cNAME: "",
        };
    },

    props: {
        id: {
            type: String,
            default: "",
        },
    },
    computed: {
        ...mapState("DataSourceModule", ["DataSourceList"]),
        ...mapGetters("API", ['urlSaveDataSource', 'urlGetDataSourceList']),
    },
    methods: {
        ...mapActions("DataSourceModule", ["RefreshDataSourceList"]),
        Save() {
            if (!this.cNAME) {
                this.$VMessage.error("请选择数据名称");
                return;
            }
            //类型判断
            if (this.cTYPE == "ORACLE") {
                if (!this.cCONNECT_STR || this.cCONNECT_STR == " ") {
                    this.$VMessage.error("请输入数据库连接信息");
                    return;
                }
            }
            if (this.cTYPE == "MapServer") {
                if (!this.cSERVER_URL || this.cSERVER_URL == " ") {
                    this.$VMessage.error("请输入服务连接信息");
                    return;
                }
            }
            if (
                this.cTYPE == "GDB" ||
                this.cTYPE == "MDB" ||
                this.cTYPE == "SHP" ||
                this.cTYPE == "SDE"
            ) {
                if (!this.cFILEPATH || this.cFILEPATH == " ") {
                    this.$VMessage.error("请输入数据源路径");
                    return;
                }
            }
            this.cFILEPATH = this.cFILEPATH || " ";
            this.cSERVER_URL = this.cSERVER_URL || " ";
            this.cCONNECT_STR = this.cCONNECT_STR || " ";
            //调用接口
            let param = {
                ID: this.id || window.guid(),
                NAME: this.cNAME,
                TYPE: this.cTYPE,
                FILEPATH: this.cFILEPATH,
                CONNECT_STR: this.cCONNECT_STR,
                SERVER_URL: this.cSERVER_URL,
            };
            let data = {
                datatsourceJSON: JSON.stringify(param),
            };

            this.$axios
                .post(this.urlSaveDataSource, data)
                .then(() => {
                    this.$VMessage.success("操作成功");
                    this.RefreshDataSourceList();
                })
                .catch((error) => {
                    this.$VMessage.error(error);
                });

            this.$emit("closewindow");
        },

        Cancel() {
            this.$emit("closewindow");
            this.RefreshDataSourceList();
        },

        //根据ID获取数据源信息
        getdata() {
            let url = `${this.urlGetDataSourceList}`;

            this.$axios.get(url).then((response) => {
                var dsinfolist = response.data.filter((ds) => {
                    return ds.ID == this.id;
                });
                this.cNAME = dsinfolist[0].NAME;
                this.cTYPE = dsinfolist[0].TYPE;
                this.cFILEPATH = dsinfolist[0].FILEPATH;
                this.cCONNECT_STR = dsinfolist[0].CONNECT_STR;
                this.cSERVER_URL = dsinfolist[0].SERVER_URL;
            });
        },
    },
    mounted() {
        if (this.id) {
            this.getdata();
        }
    },
};
</script>
