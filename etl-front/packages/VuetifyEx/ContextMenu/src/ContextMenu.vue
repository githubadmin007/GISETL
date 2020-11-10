<template>
  <div class="v-context-menu" v-show="Visible">
    <div class="menu" ref="menu">
      <slot v-bind:data="data">
        <NestList ref="nestList" v-if="NestListCfg" v-bind="NestListCfg" @itemclick="itemclick"></NestList>
      </slot>
    </div>
  </div>
</template>

<script>
import NestList from '../../NestList/src/NestList.vue'

export default {
  name: 'VContextMenu',
  components: {
    NestList
  },
  data () {
    return {
      Visible: false,
      data: null,
      NestListCfg: null // NestList配置
    }
  },
  methods: {
    Show () {
      this.Visible = true;
    },
    Hide () {
      this.Visible = false;
    },
    SetData (data) {
      this.data = data;
      if (this.$refs.nestList && this.$refs.nestList.SetData) {
        this.$refs.nestList.SetData(data);
      }
    },
    SetPosition (x, y) {
      let left = 0, top = 0;
      let pWidth = this.$el.offsetWidth;
      let sWidth = this.$refs.menu.offsetWidth;
      if (sWidth + x > pWidth) {
        left = pWidth - sWidth;
      } else {
        left = x;
      }
      let pHeight = this.$el.offsetHeight;
      let sHeight = this.$refs.menu.offsetHeight;
      if (sHeight + y > pHeight) {
        top = pHeight - sHeight;
      } else {
        top = y;
      }
      this.$refs.menu.style.left = left + 'px';
      this.$refs.menu.style.top = top + 'px';
    },
    ShowAt (x, y) {
      this.Show();
      this.$nextTick(() => {
        this.SetPosition(x, y);
      });
    },
    itemclick (event) {
      if (this.NestListCfg.click) {
        this.NestListCfg.click(event, this.data);
      }
    }
  },
  mounted () {
    this.$nextTick(function () {
      document.addEventListener('click', this.Hide);
    });
  },
  beforeDestroy () {
    document.removeEventListener('click', this.Hide);
  }
}
</script>

<style scoped>
  .v-context-menu {
    top: 0px;
    bottom: 0px;
    left: 0px;
    right: 0px;
    position: fixed;
    pointer-events: none;
    z-index:9999;
  }
    .v-context-menu .menu {
      min-width: 100px;
      min-height: 100px;
      top: 0px;
      left: 0px;
      position: fixed;
      pointer-events: auto;
    }
</style>
