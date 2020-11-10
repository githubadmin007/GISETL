/**
 * 任务模块
 */
import axios from '../../plugins/axios.js'

const TaskModule = {
    namespaced: true,
    state: {
        TaskList: [], // 任务列表
    },
    getters: {},
    mutations: {
        // 设置任务列表
        SetTaskList (state, TaskList) {
            state.TaskList = TaskList;
        }
    },
    actions: {
        // 刷新任务列表
        RefreshTaskList ({ commit, rootGetters }) {
            axios.get(rootGetters['API/urlGetTaskList']).then(response => {
                commit('SetTaskList', response.data);
            })
        }
    }
}

export default TaskModule;
