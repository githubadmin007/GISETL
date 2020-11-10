/**
 * 待执行任务模块
 */
import axios from '../../plugins/axios.js'

const TodoTaskModule = {
    namespaced: true,
    state: {
        TodoTaskList: [], // 任务列表
    },
    getters: {},
    mutations: {
        // 设置待执行任务列表
        SetTodoTaskList (state, TodoTaskList) {
            state.TodoTaskList = TodoTaskList;
        }
    },
    actions: {
        // 刷新待执行任务列表
        RefreshTodoTaskList ({ commit, rootGetters }) {
            axios.get(rootGetters['API/urlGetTodoTaskList']).then(response => {
                commit('SetTodoTaskList', response.data);
            })
        }
    }
}

export default TodoTaskModule;
