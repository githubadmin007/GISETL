/**
 * 节点模块
 */
import axios from '../../plugins/axios.js'

// 输入输出类型
const TypeItems = [
    "IFeatureClass",
    "IFeatureCursor",
    "ITable",
    "DataTable",
    "Icursor",
    "Array",
    "Any",
    "Unknown",
    "NONE"
];

const NodeModule = {
    namespaced: true,
    state: {
        NodeList: [], // 节点列表
        TypeItems
    },
    getters: {},
    mutations: {
        // 设置节点列表
        SetNodeList (state, NodeList) {
            state.NodeList = NodeList;
        }
    },
    actions: {
        // 刷新节点列表
        RefreshNodeList ({ commit, rootGetters }) {
            axios.get(rootGetters['API/urlGetNodeList']).then(response => {
                commit('SetNodeList', response.data);
            })
        }
    }
}

export default NodeModule;
