<template>
  <div class="v-nest-list">
    <v-sheet :elevation="sheetElevation" :width="sheetWidth">
      <template v-for="(item,index) in items">
        <!-- 分割线 -->
        <v-divider v-if="item==null" :key="index"></v-divider>
        <!-- 数组节点 -->
        <NestNode v-else :item="item" :data="data" :key="index" v-bind="$props" @itemclick="itemclick"></NestNode>
      </template>
    </v-sheet>
  </div>
</template>

<script>
import { isMobile } from '../../platform'
import NestNode from './NestNode'

export default {
  name: 'VNestList',
  components: {
    NestNode
  },
  data () {
    return {
      isMobile: isMobile,
      data: null
    };
  },
  props: {
    // 全部数据
    items: {
      type: Array,
      default: () => []
    },
    // 是否为根
    isRoot: {
      type: Boolean,
      default: true
    },
    // 宽度
    width: {
      type: [Number, String],
      default: '200px'
    },
    // 图标大小
    iconSize: {
      type: Number,
      default: 24
    },
    // 文字字段名
    textField: {
      type: String,
      default: 'text'
    },
    // 图标字段名
    iconField: {
      type: String,
      default: 'icon'
    },
    // 子列表字段名
    sublistField: {
      type: String,
      default: 'sublist'
    },
    // 点击回调函数字段名
    clickField: {
      type: String,
      default: 'click'
    }
  },
  computed: {
    // sheet组件的elevation值
    sheetElevation () {
      // 移动端的非根列表为0
      if (this.isMobile && !this.isRoot) {
        return 0;
      }
      return 10;
    },
    // sheet组件的宽度值
    sheetWidth () {
      // 根列表或PC端宽度始终为传入的width值
      if (this.isRoot || !this.isMobile) {
        return this.width;
      }
      return '100%';
    }
  },
  methods: {
    SetData (data) {
      this.data = data;
    },
    itemclick (event) {
      this.$emit('itemclick', event);
    }
  }
}
</script>

<style lang='scss' scoped>
  .v-nest-list {
    width: 100%;
    height: 100%;
    background-color: white;
  }
</style>
