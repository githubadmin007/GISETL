<template>
        <v-sheet style="margin:20px">
        <v-row>
            <v-col cols="4">输入名称</v-col>
            <v-col>
                <v-text-field v-model="dInput.NAME" dense outlined></v-text-field>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="4">输入别名</v-col>
            <v-col>
                <v-text-field v-model="dInput.ALIAS" dense outlined></v-text-field>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="4">输入类型</v-col>
            <v-col>
                <v-combobox v-model="dInput.TYPE" :items="TypeItems" dense outlined></v-combobox>
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
import { mapState } from 'vuex';

export default {
    data () {
        return {
            dInput: {}
        }
    },
    props: {
        // 节点
        node: {
            type: Object,
            required: true
        },
        // 节点输入对象
        input: {
            type: Object,
            default: null
        },
    },
    computed: {
        ...mapState('NodeModule', ['TypeItems']),
    },
    methods: {
        // 保存
        Save () {
            if (this.input) {
                let index = this.node.INPUTS.indexOf(this.input);
                this.node.INPUTS.splice(index, 1, this.dInput);
            } else {
                this.node.INPUTS.push(this.dInput);
            }
            this.$emit("closewindow");
        },
        // 取消
        Cancel () {
            this.$emit("closewindow");
        }
    },
    created() {
        this.dInput = this.input ? 
        { ...this.input } :
        {
            ID: window.guid(),
            NODE_ID: this.node.ID,
            NAME: '',
            ALIAS: '',
            TYPE: ''
        };
    },
}
</script>
