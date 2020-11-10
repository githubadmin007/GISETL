<template>
  <v-app>
    <!--左侧面板-->
    <v-navigation-drawer app dark>
      <!--系统列表-->
      <v-list dense nav>
        <v-list-item-icon>
          <i class="iconfont icon-guanli"></i>
        </v-list-item-icon>ETL管理
        <v-divider class="mb-1" />

        <v-list-item to="/datasource">
          <v-list-item-icon>
            <i class="iconfont icon-datasource"></i>
          </v-list-item-icon>数据源配置
        </v-list-item>
        <v-list-item to="/node">
          <v-list-item-icon>
            <i class="iconfont icon-jiedian"></i>
          </v-list-item-icon>节点管理
        </v-list-item>
        <v-list-item to="/model">
          <v-list-item-icon>
            <i class="iconfont icon-moxing"></i>
          </v-list-item-icon>模型配置
        </v-list-item>
        <v-list-item to="/task">
          <v-list-item-icon>
            <i class="iconfont icon-renwu"></i>
          </v-list-item-icon>任务管理
        </v-list-item>

        <v-list-item to="/tasklog">
          <v-list-item-icon>
            <i class="iconfont icon-caozuorizhi"></i>
          </v-list-item-icon>任务日志
        </v-list-item>
      </v-list>
    </v-navigation-drawer>
    <!--主体-->
    <v-main>
      <router-view></router-view>
    </v-main>
    <!-- 页脚 -->
    <v-footer app light padless inset>
      <v-card class="flex" flat tile>
        <v-card-text class="text-center">佛山市测绘地理信息研究院</v-card-text>
      </v-card>
    </v-footer>
  </v-app>
</template>

<script>
import { mapActions } from "vuex";

export default {
  name: "App",
  data() {
    return {};
  },
  methods: {
    ...mapActions(["InitStore"]),
  },
  mounted() {
    this.InitStore();
  },
};
</script>