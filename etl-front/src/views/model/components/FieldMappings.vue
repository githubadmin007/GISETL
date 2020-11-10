<template>
    <div>
        <v-text-field
            v-model="cName"
            label="步骤名称"
            style="width: 50%; display: inline-block"
            outlined
            dense
        ></v-text-field>
        <v-btn class="ml-2" color="primary" @click="AddField">新增字段</v-btn>
        <v-btn class="ml-2" color="primary" @click="SaveMappings">保存</v-btn>
        <v-btn class="ml-2" color="error" @click="Cancel">取消</v-btn>
        <!-- 字段列表 -->
        <v-data-table
            hide-default-footer
            :headers="dFieldTableHeader"
            :items="dFieldMappingsList"
        ></v-data-table>
        <!-- 新增字段弹窗 -->
        <v-popup-window
            v-model="cIsShowNewFieldWindow"
            v-if="cIsShowNewFieldWindow"
        >
            <v-text-field
                v-model="dNewField.SOURCE_FIELD"
                label="来源字段"
                outlined
                dense
            ></v-text-field>
            <v-text-field
                v-model="dNewField.TARGET_FIELD"
                label="目标字段"
                outlined
                dense
            ></v-text-field>
            <v-text-field
                v-model="dNewField.TARGET_ALIAS"
                label="目标字段别名"
                outlined
                dense
            ></v-text-field>
            <v-text-field
                v-model="dNewField.TARGET_TYPE"
                label="目标字段类型"
                outlined
                dense
            ></v-text-field>
            <v-text-field
                v-model="dNewField.TARGET_LENGTH"
                label="目标字段长度"
                outlined
                dense
            ></v-text-field>
            <v-btn class="ml-2" color="primary" @click="SaveNewField"
                >保存</v-btn
            >
            <v-btn class="ml-2" color="error" @click="dNewField = null"
                >取消</v-btn
            >
        </v-popup-window>
    </div>
</template>

<script>
import BaseNode from "./_BaseNode";
import { mapGetters } from "vuex";

export default {
    mixins: [BaseNode],
    data() {
        return {
            dNewField: null,
            dFieldMappingsList: [],
            dFieldTableHeader: [
                { text: "来源字段", value: "SOURCE_FIELD" },
                { text: "目标字段", value: "TARGET_FIELD" },
                { text: "目标字段别名", value: "TARGET_ALIAS" },
                { text: "目标字段类型", value: "TARGET_TYPE" },
                { text: "目标字段长度", value: "TARGET_LENGTH" },
            ],
        };
    },
    computed: {
        ...mapGetters("API", [
            "urlGetFieldMappingsList",
            "urlSaveFieldMappingsList",
        ]),
        // 是否显示新增字段窗口
        cIsShowNewFieldWindow: {
            get() {
                return this.dNewField != null;
            },
            set(value) {
                if (!value) {
                    this.dNewField = null;
                }
            },
        },
        // 映射表id参数对象
        cMappingsParam() {
            return this.cStepParamList.find(
                (sParam) => sParam.NAME === "field_group_id"
            );
        },
        // 字段映射关系组id
        cFieldGroupId() {
            return this.cMappingsParam.PARAM_VALUE || window.guid();
        },
    },
    watch: {
        cFieldGroupId() {
            this.GetFieldMappingsList();
        },
    },
    methods: {
        // 获取映射列表
        async GetFieldMappingsList() {
            let url = `${this.urlGetFieldMappingsList}?group_id=${this.cFieldGroupId}`;
            let response = await this.$axios.get(url);
            this.dFieldMappingsList = response.data;
        },
        // 新增字段
        AddField() {
            this.dNewField = {
                ID: window.guid(),
            };
        },
        // 保存新字段
        SaveNewField() {
            let field = this.dNewField;
            if (field.SOURCE_FIELD && field.TARGET_FIELD) {
                let index = this.dFieldMappingsList.findIndex(
                    (item) => item.ID === field.ID
                );
                if (index < 0) {
                    this.dFieldMappingsList.push(field);
                } else {
                    this.dFieldMappingsList.splice(index, 1, field);
                }
                this.dNewField = null;
            }
        },
        // 保存映射表
        async SaveMappings() {
            let data = {
                group_id: this.cFieldGroupId,
                mappingsJSON: JSON.stringify(this.dFieldMappingsList),
            };
            await this.$axios.post(this.urlSaveFieldMappingsList, data);
            this.cMappingsParam.PARAM_VALUE = this.cFieldGroupId;
            this.SaveStep();
        },
    },
    created() {
        if (this.cFieldGroupId) {
            this.GetFieldMappingsList();
        }
    },
};
</script>