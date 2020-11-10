<template>
    <div>
        <!-- 顶部工具条 -->
        <v-toolbar dense flat>
            <v-toolbar-title>
                <i class="iconfont icon-moxing"></i>
                模型配置
            </v-toolbar-title>
            <v-spacer></v-spacer>
            <v-text-field
                v-model="dSearch"
                color="secondary"
                hide-details
                style="max-width: 150px"
            ></v-text-field>
            <v-btn icon>
                <v-icon>mdi-magnify</v-icon>
            </v-btn>
            <v-btn icon to="/model/create">
                <v-icon>mdi-plus</v-icon>
            </v-btn>
        </v-toolbar>
        <!-- 数据列表 -->
        <v-row>
            <v-hover-col
                v-for="item in cModelList"
                :key="item.ID"
                cols="3"
                v-contextmenu="{
                    menuId: 'model-menu',
                    Data: item,
                    items: dModelMenuItems,
                }"
            >
                <v-list-item three-line>
                    <v-list-item-content>
                        <v-list-item-title>{{ item.NAME }}</v-list-item-title>
                        <v-list-item-subtitle
                            >时间：{{
                                new Date(item.TIME).Format(
                                    "yyyy-MM-dd HH:mm:ss"
                                )
                            }}</v-list-item-subtitle
                        >
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
            dModelMenuItems: [
                {
                    text: "编辑",
                    click: this.EditModel,
                },
                {
                    text: "复制",
                    click: this.CopyModel,
                },
                {
                    text: "删除",
                    click: this.DeleteModel,
                },
            ],
        };
    },
    computed: {
        ...mapState("ModelModule", ["ModelList"]),
        ...mapGetters("API", ["urlDeleteModel", "urlCopyModel"]),
        cModelList() {
            return this.ModelList.filter(
                (model) =>
                    model.NAME.toUpperCase().indexOf(
                        this.dSearch.toUpperCase()
                    ) > -1
            );
        },
    },
    methods: {
        ...mapActions("ModelModule", ["RefreshModelList"]),
        // 编辑模型
        EditModel(e, model) {
            let url = `/model/edit/${model.ID}`;
            this.$router.push(url);
        },
        // 复制模型
        async CopyModel(e, model) {
            let data = {
                model_id: model.ID,
                name: model.NAME + "_copy",
            };
            if (confirm(`是否复制${model.NAME}模型`)) {
                await this.$axios.post(this.urlCopyModel, data);
                this.$VMessage.success("复制成功");
                this.RefreshModelList();
            }
        },
        // 删除模型
        async DeleteModel(e, model) {
            let data = {
                model_id: model.ID,
            };
            if (confirm(`是否删除${model.NAME}模型`)) {
                await this.$axios.post(this.urlDeleteModel, data);
                this.$VMessage.success("删除成功");
                this.RefreshModelList();
            }
        },
    },
};
</script>
