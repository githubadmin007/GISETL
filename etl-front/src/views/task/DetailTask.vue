<template>
    <div style="margin:20px">
        <v-row>
            <v-col cols="4">开始时间</v-col>
            <v-col>
                <v-text-field v-model="cBeginTime" dense outlined disabled></v-text-field>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="4">结束时间</v-col>
            <v-col>
                <v-text-field dense outlined v-model="cEndTime" disabled></v-text-field>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="4">进度</v-col>
            <v-col>
                <v-text-field v-model="cProgress" dense outlined disabled></v-text-field>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="4">结果</v-col>
            <v-col>
                <v-text-field v-model="cResult" dense outlined disabled></v-text-field>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="4">结果信息</v-col>
            <v-col>
                <v-text-field v-model="cResultText" dense outlined disabled></v-text-field>
            </v-col>
        </v-row>
    </div>
</template>

<script>
import { mapGetters } from "vuex";
export default {
    data() {
        return {
            cBeginTime: "",
            cEndTime: "",
            cProgress: "",
            cResult: "",
            cResultText: "",
        };
    },

    props: {
        id: {
            type: String,
            default: "",
        },
    },
    computed: {
        ...mapGetters("API", ["urlGetTaskLog"]),
    },
    methods: {
        getdata() {
            let data = {
                task_id: this.id,
            };
            this.$axios
                .post(this.urlGetTaskLog, data)
                .then((response) => {
                    var sortlist = response.data;
                    if (sortlist.length == 0) return;
                    this.cBeginTime = new Date(sortlist[0].START_TIME).Format(
                        "yyyy-MM-dd HH:mm:ss"
                    );
                    this.cEndTime = new Date(sortlist[0].END_TIME).Format(
                        "yyyy-MM-dd HH:mm:ss"
                    );
                    this.cProgress = sortlist[0].PROGRESS + "%";
                    this.cResult = sortlist[0].RESULT;
                    this.cResultText = sortlist[0].RESULT_TEXT;
                })
                .catch((error) => {
                    this.$VMessage.error(error);
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