<template>
  <v-sheet>
    <v-toolbar dense flat>
      <v-toolbar-title>
        <i class="iconfont icon-caozuorizhi"></i>
        任务日志
      </v-toolbar-title>
      <v-spacer></v-spacer>
      <v-text-field
        v-model="cSearch"
        label="任务名或结果"
        color="secondary"
        hide-details
        style="max-width:150px;"
      ></v-text-field>
      <v-icon>mdi-magnify</v-icon>
      <v-btn icon @click="refresh">
        <v-icon>mdi-refresh</v-icon>
      </v-btn>
      <v-btn icon>
        <v-icon>mdi-account</v-icon>
      </v-btn>
    </v-toolbar>

    <v-data-table
      :headers="headers"
      :items="cTaskLogList"
      :items-per-page="14"
      :search="cSearch"
      class="elevation-1"
      :headers-length="7"
      :options.sync="options"
      style="margin-top:50px;"
      :footer-props="{
      showFirstLastPage: true,
      firstIcon: 'iconfont icon-daoshouye',
      lastIcon: 'iconfont icon-daoweiye',
      prevIcon: 'iconfont icon-shangyiye',
      nextIcon: 'iconfont icon-xiayiye1',
      itemsPerPageOptions:[14,28,42,-1],
      itemsPerPageText: '分页项目数',
    }"
    >
      <template v-slot:[`item.RESULT`]="{ item }">
        <v-chip :color="getColor(item.PROGRESS)" dark>{{ item.RESULT }}</v-chip>
      </template>
    </v-data-table>
    <v-footer app light padless inset>
      <v-card class="flex" flat tile>
        <v-card-text class="text-center">佛山市测绘地理信息研究院</v-card-text>
      </v-card>
    </v-footer>
  </v-sheet>
</template>


<script>
import { mapState, mapActions } from "vuex";

export default {
  data() {
    return {
      cSearch: "",
      headers: [
        { text: "ID", value: "ID", filterable: false, sortable: false },
        { text: "任务名", value: "NAME" },
        { text: "开始时间", value: "START_TIME", filterable: false },
        { text: "结束时间", value: "END_TIME", filterable: false },
        { text: "进度", value: "PROGRESS", filterable: false },
        { text: "结果", value: "RESULT" },
        {
          text: "备注",
          value: "RESULT_TEXT",
          filterable: false,
          sortable: false,
        },
      ],
      options: {
        sortBy: ["END_TIME"],
        sortDesc: ["true"],
      },
    };
  },
  computed: {
    ...mapState("TaskLogModule", ["TaskLogList"]),

    cTaskLogList: function () {
      return this.TaskLogList.map((log) => {
        log.START_TIME = new Date(log.START_TIME).Format("yyyy-MM-dd HH:mm:ss");
        log.END_TIME = new Date(log.END_TIME).Format("yyyy-MM-dd HH:mm:ss");
        log.RESULT = this.tranresult(log.RESULT);
        log.PROGRESS=log.PROGRESS+"%";
        return log;
      });
    },
  },
  methods: {
    ...mapActions("TaskLogModule", ["RefreshTaskLogList"]),
    refresh() {
      this.RefreshTaskLogList()
        .then(() => {
          this.$VMessage.success("日志已更新");
        })
        .catch((error) => {
          this.$VMessage.error(error);
        });
    },

    //1 -1 to 成功失败
    tranresult(result) {
      if (result == 1) {
        return "成功";
      } else if (result == -1) {
        return "失败";
      }
    },

    //根据进度为结果设置背景色
    getColor(resulttype) {
      resulttype=resulttype.replace("%","");//去除之前添加的% 进行结果背景色判断 
      if (resulttype == 100) return "green";
      else if (resulttype == 0) return "red";
      else return "orange";
    },
  },

  created() {
    // 进入页面时 自动刷新数据
    this.RefreshTaskLogList();
  },
};
</script>