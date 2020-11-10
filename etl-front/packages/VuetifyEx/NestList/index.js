import NestList from './src/NestList.vue'

NestList.install = function (Vue) {
  Vue.component(NestList.name, NestList);
}

export default NestList
