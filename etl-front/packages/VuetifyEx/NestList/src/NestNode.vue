<template>
  <!-- 数据项 -->
  <div class="v-nest-node" :width="width">
    <!-- 节点容器 -->
    <div class="node-panel" @mouseenter.prevent="ShowSubList" @mouseleave="HideSubList" @click="NodeClick">
      <!-- 图标 -->
      <v-avatar :size="iconSize">
        <v-img :src="item[iconField]"></v-img>
      </v-avatar>
      <!-- 文字 -->
      <div class="content" :style="[contentStyle]">
        {{ content }}
      </div>
      <!-- 如果有子列表 -->
      <v-icon v-show="hasSubList">{{ arrowIcon }}</v-icon>
    </div>
    <!-- 子列表容器 -->
    <div v-if="NodeOpen"
      :class="['sub-list-panel'+(isMobile?'-mobile':'-pc')]"
      @mouseenter="ShowSubList"
      @mouseleave="HideSubList"
      ref="SubList">
      <v-nest-list class="sub-list" v-bind="$props" :items="subList" :is-root="false" @itemclick="itemclick"></v-nest-list>
    </div>
  </div>
</template>

<script>
import { isMobile } from '../../platform'

export default {
  data () {
    return {
      NodeOpen: false, // 节点是否展开
      isMobile: isMobile
    };
  },
  props: {
    // 数据对象
    item: {
      type: Object
    },
    // 附加数据
    data: {
      type: [Object, Number, String, Array],
      default: null
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
    // 图标图片路径
    iconSrc () {
      if (this.item && this.item[this.iconField]) {
        return this.item[this.iconField];
      }
      return '';
    },
    // 文字内容
    content () {
      if (this.item && this.item[this.textField]) {
        return this.item[this.textField];
      }
      return '';
    },
    // 是否有子列表
    hasSubList () {
      if (this.item && this.item[this.sublistField]) {
        return this.item[this.sublistField].length > 0;
      }
      return false;
    },
    // 子列表数据
    subList () {
      return this.item[this.sublistField];
    },
    // 点击回调函数
    click () {
      return this.item[this.clickField];
    },
    // 内容样式
    contentStyle () {
      return {
        width: 'calc(100% - ' + (this.iconSize + (this.hasSubList ? 34 : 10)) + 'px)'
      };
    },
    // 箭头图标
    arrowIcon () {
      if (this.isMobile) {
        return this.NodeOpen ? 'mdi-chevron-down' : 'mdi-chevron-right';
      }
      return 'mdi-chevron-right';
    }
  },
  methods: {
    // 打开子列表（PC端）
    ShowSubList () {
      if (this.hasSubList && !this.isMobile) {
        this.NodeOpen = true;
        this.$nextTick(() => {
          let SubList = this.$refs.SubList;
          if (SubList) {
            // 获取屏幕大小
            let screenRect = this.$root.$el.getBoundingClientRect();
            // 父节点大小
            let parentRect = this.$el.getBoundingClientRect();
            // 子列表大小
            let sublstRect = SubList.getBoundingClientRect();
            // 确定水平位置
            if (parentRect.right + sublstRect.width > screenRect.width) {
              SubList.style.left = (-sublstRect.width) + 'px';
            } else {
              SubList.style.left = parentRect.width + 'px';
            }
            // 确定垂直位置
            if (parentRect.top + sublstRect.height > screenRect.height) {
              SubList.style.top = (this.$el.offsetTop + parentRect.height - sublstRect.height) + 'px';
            } else {
              SubList.style.top = this.$el.offsetTop + 'px';
            }
          }
        });
      }
    },
    // 关闭子列表（PC端）
    HideSubList () {
      if (!this.isMobile) {
        this.NodeOpen = false;
      }
    },
    // 节点点击事件
    NodeClick (event) {
      if (this.hasSubList) {
        // 移动端点击有子列表的节点，toggle列表
        if (this.isMobile) {
          this.NodeOpen = !this.NodeOpen;
        }
        event.stopPropagation();
      } else {
        event.item = this.item;
        // 优先触发数据中带的回调函数
        if (this.click) {
          this.click(event, this.data);
        } else {
          this.$emit('itemclick', event);
        }
      }
    },
    // 子列表的节点点击事件
    itemclick (event) {
      this.$emit('itemclick', event);
    }
  }
}
</script>

<style lang='scss' scoped>
  .v-nest-node {
    /* 节点 */
    .node-panel {
      width: 100%;
      padding-left: 5px;
      &:hover {
        background-color: #ddd;
      }
      .content {
        display: inline-block;
        padding: 10px;
        cursor: default;
      }
    }
    /* 子列表（移动） */
    .sub-list-panel-mobile {
      margin-left: 24px;
      width: calc(100% - 24px);
      .sub-list {
        color: green;
      }
    }
    /* 子列表（PC） */
    .sub-list-panel-pc {
      position: absolute;
      .sub-list {
        color: blue;
      }
    }
  }
</style>