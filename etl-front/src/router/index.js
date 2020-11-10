import Vue from 'vue'
import VueRouter from 'vue-router'

Vue.use(VueRouter)

const routes = [
    {
        path: '/',
        name: 'Home',
        component: () => import('../views/datasource/manager.vue')
    },
    // 数据源管理
    {
        path: '/datasource',
        name: 'datasource',
        component: () => import('../views/datasource/manager.vue')
    },
    // 节点管理
    {
        path: '/node',
        name: 'node',
        component: () => import('../views/node/manager.vue')
    },
    {
        path: '/node/create',
        name: 'CreateNode',
        component: () => import('../views/node/NodeEditor.vue')
    },
    {
        path: '/node/edit/:id',
        name: 'EditNode',
        props: true,
        component: () => import('../views/node/NodeEditor.vue')
    },
    // 模型管理
    {
        path: '/model',
        name: 'Model',
        component: () => import('../views/model/manager.vue')
    },
    {
        path: '/model/create',
        name: 'CreateModel',
        component: () => import('../views/model/ModelEditor.vue')
    },
    {
        path: '/model/edit/:id',
        name: 'EditModel',
        props: true,
        component: () => import('../views/model/ModelEditor.vue')
    },
    
    // 任务相关
    {
        path: '/task',
        name: 'Task',
        component: () => import('../views/task/manager.vue')
    },
    {
        path: '/task/create',
        name: 'CreateTask',
        component: () => import('../views/task/taskInfo.vue')
    },
    {
        path: '/task/edit/:id',
        name: 'EditTask',
        props: true,
        component: () => import('../views/task/taskInfo.vue')
    },


    //任务日志相关
    {
        path: '/tasklog',
        name: 'TaskLog',
        component: () => import('../views/log/manager.vue')
    },

]

const router = new VueRouter({
    mode: 'history',
    base: process.env.BASE_URL,
    routes
})

export default router
