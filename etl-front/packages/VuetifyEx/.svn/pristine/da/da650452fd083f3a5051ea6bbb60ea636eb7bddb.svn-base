<template>
  <transition name="v-message-fade" @after-leave="handleAfterLeave">
    <div
      :class="[
        'v-message',
        center ? 'is-center' : '',
        showClose ? 'is-closable' : '',
        customClass
      ]"
      :style="[ positionStyle, colorStyle ]"
      v-show="visible"
      @mouseenter="clearTimer"
      @mouseleave="startTimer">
      <v-icon dense class="v-message__icon" :style="colorStyle">{{ iconType }}</v-icon>
      <slot>
        <p v-if="dangerouslyUseHTMLString" v-html="message" class="v-message__content"></p>
        <p v-else class="v-message__content">{{ message }}</p>
      </slot>
      <v-icon dense v-if="showClose" class="v-message__closeBtn" :style="colorStyle" @click="close">mdi-close</v-icon>
    </div>
  </transition>
</template>

<script type="text/babel">

const color = {
  // 背景色、边框色、前景色
  success: ['#f0f9eb', '#e1f3d8', '#67C23A'],
  info: ['#edf2fc', '#EBEEF5', '#909399'],
  warning: ['#fdf6ec', '#faecd8', '#E6A23C'],
  error: ['#fef0f0', '#fde2e2', '#F56C6C']
}

export default {
  name: 'v-message',
  data () {
    return {
      visible: false,
      message: '', // 消息文字
      duration: 2000, // 显示时间, 毫秒。设为 0 则不会自动关闭
      type: 'info', // 主题
      customClass: '', // 自定义类名
      onClose: null, // 关闭时的回调函数, 参数为被关闭的 message 实例
      showClose: true, // 是否显示关闭按钮
      closed: false,
      verticalOffset: 20, // Message 距离窗口顶部的偏移量
      timer: null,
      dangerouslyUseHTMLString: false, // 是否将 message 属性作为 HTML 片段处理
      center: false // 文字是否居中
    };
  },
  computed: {
    // 弹窗位置
    positionStyle () {
      return {
        'top': this.verticalOffset + 'px'
      };
    },
    // 颜色配置
    colorStyle () {
      return {
        'background-color': color[this.type][0],
        'border-color': color[this.type][1],
        'color': color[this.type][2]
      }
    },
    // 图标类型
    iconType () {
      switch (this.type) {
        case 'success' : return 'mdi-check-circle';
        case 'info' : return 'mdi-information';
        case 'warning' : return 'mdi-alert-circle';
        case 'error' : return 'mdi-close-circle';
      }
      return 'mdi-information';
    }
  },
  watch: {
    closed (newVal) {
      if (newVal) {
        this.visible = false;
      }
    }
  },
  methods: {
    handleAfterLeave () {
      this.$destroy(true);
      this.$el.parentNode.removeChild(this.$el);
    },

    close () {
      this.closed = true;
      if (typeof this.onClose === 'function') {
        this.onClose(this);
      }
    },

    clearTimer () {
      clearTimeout(this.timer);
    },

    startTimer () {
      if (this.duration > 0) {
        this.timer = setTimeout(
          () => {
            if (!this.closed) {
              this.close();
            }
          },
          this.duration
        );
      }
    },
    // 键盘事件
    keydown (e) {
      if (e.keyCode === 27) { // esc关闭消息
        if (!this.closed) {
          this.close();
        }
      }
    }
  },
  mounted () {
    this.startTimer();
    document.addEventListener('keydown', this.keydown);
  },
  beforeDestroy () {
    document.removeEventListener('keydown', this.keydown);
  }
};
</script>

<style lang='scss' scoped>
  /*组件根节点样式*/
  .v-message {
    min-width: 380px;
    max-width: 50%;
    -webkit-box-sizing: border-box;
    box-sizing: border-box;
    border-width: 1px;
    border-style: solid;
    position: fixed;
    left: 50%;
    top: 20px;
    -webkit-transform: translateX(-50%);
    transform: translateX(-50%);
    -webkit-transition: opacity .3s,top .4s,-webkit-transform .4s;
    transition: opacity .3s,top .4s,-webkit-transform .4s;
    transition: opacity .3s,transform .4s,top .4s;
    transition: opacity .3s,transform .4s,top .4s,-webkit-transform .4s;
    padding: 15px 15px 15px 20px;
    display: -webkit-box;
    display: -ms-flexbox;
    display: flex;
    -webkit-box-align: center;
    -ms-flex-align: center;
    align-items: center;
  }
  /*组件根节点样式（内容居中）*/
  .is-center {
    -webkit-box-pack: center;
    -ms-flex-pack: center;
    justify-content: center;
  }
  /*组件根节点样式（可关闭）*/
  .is-closable {
    .v-message__content {
      padding-right: 25px;
    }
  }

  /*图标样式*/
  .v-message__icon {
    margin-right: 10px;
  }

  /*文字样式*/
  .v-message__content {
    padding: 0;
    font-size: 14px;
    line-height: 1;
    p {
      margin: 0;
    }
  }

  // 关闭按钮
  .v-message__closeBtn {
    position: absolute!important;
    top: 50%;
    right: 15px;
    -webkit-transform: translateY(-50%);
    transform: translateY(-50%);
    cursor: pointer;
    font-size: 16px;
  }
</style>
