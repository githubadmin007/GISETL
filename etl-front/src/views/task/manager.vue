<template>
    <v-sheet>
        <!-- 顶部工具条 -->
        <v-toolbar dense flat>
            <v-toolbar-title>
                <i class="iconfont icon-renwu"></i>
                任务管理
            </v-toolbar-title>
            <v-spacer></v-spacer>
            <v-text-field v-model="dSearch" color="secondary" hide-details style="max-width:150px;"></v-text-field>
            <v-btn icon to="/task/create">
                <v-icon>mdi-plus</v-icon>
            </v-btn>
            <v-btn icon>
                <v-icon>mdi-account</v-icon>
            </v-btn>
        </v-toolbar>
        <!-- 数据列表 -->
        <v-row>
            <v-hover-col
                v-for="item in cTaskList"
                :key="item.ID"
                cols="3"
                v-contextmenu="{menuId:'task-menu',Data:item,items:dTaskMenuItems}"
            >
                <v-list-item three-line>
                    <v-list-item-content>
                        <v-list-item-title>{{item.NAME}}</v-list-item-title>
                        <v-list-item-subtitle style="margin-top:8px; ">状态：{{item.state}}</v-list-item-subtitle>
                        <v-list-item-subtitle style="margin-top:8px;">重复模式：{{item.repeat}}</v-list-item-subtitle>
                        <v-list-item-subtitle>
                            最近一次执行时间：{{time(item.LAST_TIME)}}
                            <v-btn
                                dense
                                class="ml-2"
                                tile
                                outlined
                                color="#01579B"
                                @click="detail(item)"
                            >详情</v-btn>
                        </v-list-item-subtitle>
                        <v-list-item-subtitle>
                            <v-btn
                                class="my-2"
                                tile
                                outlined
                                color="#388E3C"
                                :loading="item.loading"
                                @click="execute(item)"
                            >执行</v-btn>
                        </v-list-item-subtitle>
                    </v-list-item-content>
                </v-list-item>
            </v-hover-col>
        </v-row>
    </v-sheet>
</template>

<script>
import { mapState, mapGetters, mapActions } from "vuex";
import TaskdetailInfo from "./DetailTask";
export default {
    data() {
        return {
            page: 1,
            dSearch: "",
            timer: null, //定时器名称
            dTaskMenuItems: [
                {
                    text: "编辑",
                    click: this.EditTask,
                },
                {
                    text: "删除",
                    click: this.DeleteTask,
                },
            ],
        };
    },
    computed: {
        ...mapState("TaskModule", ["TaskList"]),
        ...mapState("TodoTaskModule", ["TodoTaskList"]), //获取执行任务列表
        ...mapGetters("API", [
            "urlDeleteTask",
            "urlSaveTodoTask",
            "urlGetTodoTaskID",
        ]),

        //任务列表
        cTaskList: function () {
            return this.TaskList.filter((task) => {
                return (
                    task.NAME.toUpperCase().indexOf(
                        this.dSearch.toUpperCase()
                    ) > -1
                );
            }).map((task) => {
                task.loading =
                    this.TodoTaskList.findIndex(
                        (value) => value.TASK_ID == task.ID
                    ) > -1;
                task.repeat = this.isrepeat(task);
                task.state = this.isstate(task);
                return task;
            });
        },
    },

    watch: {
        TodoTaskList(list, oldlist) {
            if (list.length > 0 && !this.timer) {
                //创建timeer onTime执行定时器里的函数 刷新数据
                this.timer = setInterval(this.onTime, 1000);
            }
            if (list.length == 0 && this.timer) {
                // 清除timer
                window.clearInterval(this.timer);
                this.timer = null;
            }

            if (oldlist.length > list.length) {
                this.$VMessage.success(
                    "当前任务执行完毕，请点击详情按钮或日志模块查看结果"
                );
            }
        },
    },
    methods: {
        ...mapActions("TaskModule", ["RefreshTaskList"]),
        ...mapActions("TodoTaskModule", ["RefreshTodoTaskList"]),
        // 编辑任务
        EditTask(e, task) {
            let url = `/task/edit/${task.ID}`;
            this.$router.push(url);
        },
        // 删除任务
        async DeleteTask(e, task) {
            let data = {
                task_id: task.ID,
            };
            if (confirm(`是否删除${task.NAME}任务`)) {
                this.$axios
                    .post(this.urlDeleteTask, data)
                    .then(() => {
                        this.$VMessage.success("删除成功");
                        this.RefreshTaskList();
                    })
                    .catch((error) => {
                        this.$VMessage.error(error);
                    });
            }
        },

        //判断状态
        isstate(task) {
            var state = task.STATE;
            if (state == 1) {
                return "正常";
            } else if (state == 0) {
                return "停用";
            }
        },
        //判断执行情况
        isrepeat(Item) {
            var repeatmode = Item.REPEAT_MODE;
            if (repeatmode == 1) {
                //按执行时间
                var timeregular = Item.TIME_REGULAR;
                return `${timeregular} 执行一次`;
            } else if (repeatmode == 2) {
                var timeinterval = Item.TIME_INTERVAL;
                return `每隔 ${timeinterval} 分钟执行一次`;
            } else if (repeatmode == 3) {
                return `只执行一次`;
            }
            return repeatmode;
        },
        //详细信息
        detail(Item) {
            this.$VWindow({
                id: "detailtaskInfo",
                title: "任务详情",
                moveAble: true,
                component: TaskdetailInfo,
                componentProps: {
                    id: Item.ID,
                },
            });
        },

        //执行任务
        execute(task) {
            let taskid = task.ID;
            let param = {
                ID: window.guid(),
                TASK_ID: taskid,
                TASK_NAME: task.NAME,
                ADD_TIME: new Date().Format("yyyy/MM/dd HH:mm:ss"),
                FINISHED_STATE: 0,
            };
            let data = {
                todotaskJSON: JSON.stringify(param),
            };

            this.$axios
                .post(this.urlSaveTodoTask, data)
                .then((response) => {
                    //数据保存入库后 刷新待执行任务列表 获取实时未执行任务
                    this.RefreshTodoTaskList();
                    console.log(response);
                })
                .catch((error) => {
                    this.$VMessage.error(error);
                });
        },

        //定时刷新
        onTime() {
            this.RefreshTodoTaskList();
            this.RefreshTaskList();
        },

        //时间格式转换
        time(tt) {
            var trantime = new Date(tt).Format("yyyy-MM-dd HH:mm:ss");
            return trantime;
        },
    },
    mounted() {
        this.RefreshTodoTaskList();
    },
};
</script>
