<template>
    <div style="margin:20px">
        <!-- 步骤名称 -->
        <v-row>
            <v-col cols="2">步骤名称</v-col>
            <v-col>
                <v-text-field v-model="cName" dense outlined></v-text-field>
            </v-col>
        </v-row>
        <!-- 步骤操作 -->
        <v-row>
            <v-col cols="2">步骤操作</v-col>
            <v-col>
                <v-select
                    dense
                    outlined
                    v-model="cNodeId"
                    :items="NodeList"
                    item-text="NAME"
                    item-value="ID"
                ></v-select>
            </v-col>
        </v-row>
        <!-- 参数列表 -->
        <v-row v-for="param in cStepParamList" :key="param.ID">
            <v-col>
                <span v-if="param.REQUIRED==='是'" style="color:#f00;">*</span>
                {{param.ALIAS}}
            </v-col>
            <v-col>
                <v-text-field v-model="param.PARAM_VALUE" dense outlined></v-text-field>
            </v-col>
            <v-col>是否为动态参数</v-col>
            <v-col>
                <v-select dense outlined v-model="param.IS_DYNAMIC" :items="['是','否']"></v-select>
            </v-col>
        </v-row>
        <!-- 保存、取消 -->
        <v-row>
            <v-col>
                <v-btn class="float-right mr-5" color="error" @click="Cancel">取消</v-btn>
                <v-btn class="float-right mr-5" color="primary" @click="SaveStep">保存</v-btn>
            </v-col>
        </v-row>
    </div>
</template>

<script>
import BaseNode from './components/_BaseNode'

export default {
    mixins: [BaseNode]
};
</script>