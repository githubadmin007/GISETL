<template>
    <div>
        <!-- 顶部工具条 -->
        <v-toolbar dense flat>
            <v-toolbar-title>
                <i class="iconfont icon-jiedian"></i>
                节点管理
            </v-toolbar-title>
            <v-spacer></v-spacer>
            <v-text-field v-model="dSearch" color="secondary" hide-details style="max-width:150px;"></v-text-field>
            <v-btn icon>
                <v-icon>mdi-magnify</v-icon>
            </v-btn>
            <v-btn icon to="/node/create">
                <v-icon>mdi-plus</v-icon>
            </v-btn>
        </v-toolbar>
        <!-- 数据列表 -->
        <v-row>
            <v-hover-col
                v-for="item in cNodeList"
                :key="item.ID"
                cols="3"
                v-contextmenu="{menuId:'node-menu',Data:item,items:dNodeMenuItems}"
            >
                <v-list-item three-line>
                    <v-list-item-content>
                        <v-list-item-title>{{item.NAME}}</v-list-item-title>
                        <v-list-item-subtitle class="mt-3">节点类名：{{item.CLASS_NAME}}</v-list-item-subtitle>
                        <v-list-item-subtitle class="mt-3">输出要素类型：{{item.OUTPUT_TYPE}}</v-list-item-subtitle>
                    </v-list-item-content>
                </v-list-item>
            </v-hover-col>
        </v-row>
    </div>
</template>

<script>
import { mapState, mapGetters, mapActions } from "vuex";

export default {
    data() {
        return {
            dSearch: "",
            dNodeMenuItems: [
                {
                    text: "编辑",
                    click: this.EditNode,
                },
                {
                    text: "删除",
                    click: this.DeleteNode,
                },
            ],
        };
    },
    computed: {
        ...mapState("NodeModule", ["NodeList"]),
        ...mapGetters("API", ["urlDeleteNode"]),
        cNodeList() {
            return this.NodeList.filter(
                (node) =>
                    node.NAME.toUpperCase().indexOf(
                        this.dSearch.toUpperCase()
                    ) > -1
            );
        },
    },
    methods: {
        ...mapActions("NodeModule", ["RefreshNodeList"]),
        // 编辑节点
        EditNode(e, node) {
            let url = `/node/edit/${node.ID}`;
            this.$router.push(url);
        },
        // 删除节点
        async DeleteNode(e, node) {
            let url = `${this.urlDeleteNode}?NodeId=${node.ID}`;
            if (confirm(`是否删除节点:${node.NAME}`)) {
                await this.$axios.post(url);
                this.$VMessage.success("删除成功");
                this.RefreshNodeList();
            }
        },
    },
};
</script>
