import Vue from 'vue'
import Vuex from 'vuex'
import API from './modules/API'
import DataSourceModule from './modules/DataSourceModule'
import NodeModule from './modules/NodeModule'
import ModelModule from './modules/ModelModule'
import TaskModule from './modules/TaskModule'
import TodoTaskModule from './modules/TodoTaskModule'
import TaskLogModule from './modules/TaskLogModule'

Vue.use(Vuex)

export default new Vuex.Store({
    state: {
    },
    mutations: {
    },
    actions: {
        // 初始化vuex存储
        InitStore({ dispatch }) {
            dispatch('DataSourceModule/RefreshDataSourceList'); // 刷新数据源列表
            dispatch('NodeModule/RefreshNodeList'); // 刷新节点列表
            dispatch('ModelModule/RefreshModelList'); // 刷新模型列表
            dispatch('TaskModule/RefreshTaskList'); // 刷新任务列表
            dispatch('TodoTaskModule/RefreshTodoTaskList'); // 刷新待执行任务列表
            dispatch('TaskLogModule/RefreshTaskLogList'); // 刷新任务日志列表
        }
    },
    modules: {
        API,
        DataSourceModule,
        NodeModule,
        ModelModule,
        TaskModule,
        TodoTaskModule,
        TaskLogModule
    }
})
