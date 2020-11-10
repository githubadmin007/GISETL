<template>
    <div>
        <!-- 工具条 -->
        <v-toolbar dense flat>
            <v-toolbar-title>
                <i class="iconfont icon-datasource"></i>数据源配置
            </v-toolbar-title>
            <v-spacer></v-spacer>
            <v-text-field v-model="cSearch" color="secondary" hide-details style="max-width:150px;"></v-text-field>
            <v-btn icon @click="SearchDataSource">
                <v-icon>mdi-magnify</v-icon>
            </v-btn>
            <v-btn icon @click="NewDataSource">
                <v-icon>mdi-plus</v-icon>
            </v-btn>
            <v-btn icon>
                <v-icon>mdi-account</v-icon>
            </v-btn>
        </v-toolbar>
        <!-- 数据列表 -->
        <v-data-iterator
            :items="FDataSourceList"
            style="margin-top:15px"
            :items-per-page="FDataSourceList.length"
            :footer-props="{
                showFirstLastPage: true,
                firstIcon: 'iconfont icon-daoshouye',
                lastIcon: 'iconfont icon-daoweiye',
                prevIcon: 'iconfont icon-shangyiye',
                nextIcon: 'iconfont icon-xiayiye1',
                itemsPerPageOptions:[-1,10,15,20],
            }"
        >
            <template v-slot:default="{items}">
                <v-row>
                    <v-hover-col
                        cols="12"
                        sm="6"
                        md="4"
                        lg="4"
                        v-for="item in items"
                        :key="item.ID"
                        v-contextmenu="{menuId:'ds-menu',Data:item,items:dDsMenuItems}"
                    >
                        <v-list-item three-line>
                            <v-list-item-content>
                                <v-list-item-title>{{item.NAME}}</v-list-item-title>
                                <v-list-item-subtitle>类型：{{item.TYPE}}</v-list-item-subtitle>
                                <v-list-item-subtitle>{{item.source}}</v-list-item-subtitle>
                            </v-list-item-content>
                        </v-list-item>
                    </v-hover-col>
                </v-row>
            </template>
        </v-data-iterator>
    </div>
</template>

<script>
import { mapState, mapGetters, mapActions } from "vuex";
import datasourceInfo from "./datasourceInfo";
export default {
    data() {
        return {
            page: 1,
            cSearch: "",
            ctype: "",
            dDsMenuItems: [
                {
                    text: "编辑",
                    click: this.EditIDDS,
                },
                {
                    text: "删除",
                    click: this.DeleteDS,
                },
            ],
        };
    },
    computed: {
        ...mapState("DataSourceModule", ["DataSourceList"]),
        ...mapGetters("API", ["urlDeleteDataSource"]),

        cDataSourceList: function () {
            return this.DataSourceList.map((datas) => {
                datas.source = this.issourcetype(datas);
                return datas;
            });
        },

        FDataSourceList: function () {
            var fDataSourceList = this.cDataSourceList.filter((s) => {
                return (
                    s.NAME.toUpperCase().indexOf(this.cSearch.toUpperCase()) !=
                    -1
                );
            });
            return fDataSourceList;
        },
    },
    methods: {
        ...mapActions("DataSourceModule", ["RefreshDataSourceList"]),

        //新增数据源
        NewDataSource() {
            let datasource = {
                ID: "",
                NAME: "",
            };
            this.$VWindow({
                id: "newdatasourceInfo",
                title: "新建数据源", //新建数据源建议与编辑页面分来
                moveAble: true,
                component: datasourceInfo,
                componentProps: {
                    datasource,
                },
            });
            this.RefreshDataSourceList();
        },

        //删除数据源
        async DeleteDS(e, datasource) {
            let url = `${this.urlDeleteDataSource}?datasoureid=${datasource.ID}`;
            if (confirm(`是否删除${datasource.NAME}数据源`)) {
                this.$axios
                    .post(url)
                    .then(() => {
                        this.$VMessage.success("删除成功");
                        this.RefreshDataSourceList();
                    })
                    .catch((error) => {
                        this.$VMessage.error(error);
                    });
            }
        },

        //编辑 传入ID 从数据库中调取内容 以保证内容实时更新
        EditIDDS(e, datasource) {
            this.$VWindow({
                id: "datasourceInfo",
                title: "编辑数据源",
                moveAble: true,
                component: datasourceInfo,
                componentProps: {
                    id: datasource.ID,
                },
            });
            this.RefreshDataSourceList();
        },

        //搜索数据源  输入全称时点击收缩按钮 默认进入编辑状态
        SearchDataSource() {
            if (this.cSearch) {
                var searchid = "";
                this.DataSourceList.forEach((element) => {
                    if (element.NAME == this.cSearch) {
                        searchid = element.ID;
                    }
                });
                if (searchid) {
                    this.$VWindow({
                        id: "datasourceInfo",
                        title: "搜索数据源",
                        moveAble: true,
                        component: datasourceInfo,
                        componentProps: {
                            id: searchid,
                        },
                    });
                } else {
                    alert("请确认数据源全称是否正确或数据源是否存在");
                }
            } else {
                alert("请输入搜索内容");
            }
        },

        //根据类型判断数据源信息
        issourcetype(Item) {
            var type = Item.TYPE;
            if (
                type === "GDB" ||
                type === "MDB" ||
                type === "SHP" ||
                type === "SDE"
            ) {
                return `数据源：${Item.FILEPATH}`;
            } else if (type === "ORACLE") {
                //剔除密码字段
                var strlist = Item.CONNECT_STR.split(";");
                var psvalue = "";
                strlist.forEach((element) => {
                    if (element.toUpperCase().indexOf("PASSWORD") != -1) {
                        psvalue = element;
                    }
                });
                strlist.remove(psvalue);
                var str = strlist.join(" ; ");

                return `数据库连接信息：${str}`;
            } else if (type === "MapServer") {
                return `服务连接信息：${Item.SERVER_URL}`;
            }
        },
    },
};
</script>