<template>
    <div>
        <!-- 选择输入 -->
        <v-row>
            <v-col>
                <v-select
                    dense
                    outlined
                    v-model="SelectId"
                    :items="inputs"
                    item-text="NAME"
                    item-value="ID"
                ></v-select>
            </v-col>
        </v-row>
        <!-- 保存、取消 -->
        <v-row>
            <v-col>
                <v-btn class="float-right mr-5" color="error" @click="Cancel">取消</v-btn>
                <v-btn class="float-right mr-5" color="primary" @click="Save">保存</v-btn>
            </v-col>
        </v-row>
    </div>
</template>

<script>
export default {
    data() {
        return {
            SelectId: "",
        };
    },
    props: {
        inputs: {
            type: Array,
            default: () => [],
        },
        callback: Function,
    },
    computed: {
        cInput() {
            return this.inputs.find((input) => input.ID == this.SelectId);
        },
    },
    methods: {
        // 保存
        Save() {
            if (this.callback) {
                this.callback(this.cInput);
            }
            this.$emit("closewindow");
        },
        // 取消
        Cancel() {
            if (this.callback) {
                this.callback(null);
            }
            this.$emit("closewindow");
        },
    },
};
</script>
