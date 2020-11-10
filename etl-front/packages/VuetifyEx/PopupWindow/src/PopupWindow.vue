<template>
    <div :id="id" v-if="dShow" class="popup-window-container">
        <div
            class="popup-window-shade"
            :style="[cShadeStyle]"
            v-if="!dIsMinimize"
            @click="ShadeClick"
        ></div>
        <div
            :class="['popup-window-window', `elevation-${elevation}`]"
            :style="[cPositionStyle]"
        >
            <!-- 头部 -->
            <div
                class="popup-window-head"
                :style="[cTitleStyle]"
                @mousedown="TitleMouseDown"
            >
                <!-- 图标 -->
                <div class="icon" v-if="icon || iconimg">
                    <v-icon v-if="icon">{{ icon }}</v-icon>
                    <v-avatar v-else :size="30" tile>
                        <v-img :src="iconimg"></v-img>
                    </v-avatar>
                </div>
                <!-- 标题 -->
                <div class="title" :title="title">{{ title }}</div>
                <!-- 按钮 -->
                <div class="btns">
                    <v-icon class="btn" v-if="cMinimize" @click="Minimize"
                        >mdi-window-minimize</v-icon
                    >
                    <v-icon class="btn" v-if="cNormalizate" @click="Maximize">
                        {{
                            (dIsMinimize && !dIsMaximize) ||
                            (!dIsMinimize && dIsMaximize)
                                ? "mdi-window-restore"
                                : "mdi-window-maximize"
                        }}
                    </v-icon>
                    <v-icon class="btn" v-if="closeAble" @click.stop="Close"
                        >mdi-window-close</v-icon
                    >
                </div>
            </div>
            <!-- 内容 -->
            <div v-show="!dIsMinimize" class="popup-window-content">
                <slot>
                    <iframe v-if="src" :src="src"></iframe>
                    <component
                        v-else-if="component"
                        v-bind:is="component"
                        v-bind="componentProps"
                        @closewindow="Close"
                    >
                    </component>
                </slot>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    name: "VPopupWindow",
    data() {
        return {
            dTopMoved: null, // 移动后的top
            dLeftMoved: null, // 移动后的left
            dIsMaximize: false, // 是否处于最大化状态
            dIsMinimize: false, // 是否处于最小化状态
            dShow: this.value, // 窗体是否可见，value是参数不能直接修改
        };
    },
    props: {
        // id,
        id: {
            type: String,
        },
        // 是否显示窗体
        value: {
            type: Boolean,
            default: false,
        },
        // 标题
        title: {
            type: String,
        },
        // 图标
        icon: {
            type: String,
        },
        // 图标图片
        iconimg: {
            type: String,
        },
        // 宽
        width: {
            type: String,
            default: "50%",
        },
        // 高
        height: {
            type: String,
            default: "50%",
        },
        // 弹窗位置
        position: {
            type: [String, Array],
            default: "auto",
        },
        // 外间距（仅在position为r、b、l等字符时生效）
        margin: {
            type: [String, Array],
            default: "10px",
        },
        // 允许最小化
        minimize: {
            type: Boolean,
            default: false,
        },
        // 允许最大化
        maximize: {
            type: Boolean,
            default: false,
        },
        // 关闭按钮
        closeAble: {
            type: Boolean,
            default: true,
        },
        // 允许拖动
        moveAble: {
            type: Boolean,
            default: false,
        },
        // 全屏
        fullscreen: {
            type: Boolean,
            default: false,
        },
        // 要打开的链接
        src: {
            type: String,
            default: null,
        },
        // 组件
        component: {
            type: [Object, Function],
            default: null,
        },
        // 组件参数
        componentProps: {
            type: Object,
            default: () => {},
        },
        // 遮罩
        shade: {
            type: [String, Number],
            default: 0,
        },
        // 点击遮罩是否关闭(仅在shadeEvent为true时生效)
        shadeClose: {
            type: Boolean,
            default: false,
        },
        // 是否屏蔽鼠标事件
        shadeEvent: {
            type: Boolean,
            default: false,
        },
        // 关闭前的回调
        beforeClose: {
            type: Function,
        },
        // 窗体悬浮高度
        elevation: {
            type: Number,
            default: 2,
        },
    },
    computed: {
        // 外间距（按css的规则进行处理，返回一个长度为4的数组）
        cMargin() {
            let arr = new Array(4);
            let margin = this.margin;
            if (typeof margin === "object") {
                switch (margin.length) {
                    case 1:
                        arr.fill(margin[0]);
                        break;
                    case 2:
                        arr[0] = arr[2] = margin[0];
                        arr[1] = arr[3] = margin[1];
                        break;
                    default:
                        arr.map((item, index) => {
                            arr[index] = margin[index] || "0px";
                        });
                        break;
                }
            } else {
                arr.fill(margin);
            }
            return arr;
        },
        // 宽度
        cWidth() {
            return this.width;
        },
        // 高度
        cHeight() {
            return this.height;
        },
        // 窗体位置--top
        cTop() {
            if (this.dTopMoved) return this.dTopMoved;
            let position = this.position;
            if (typeof position === "object") {
                if (position.length === 2) {
                    return position[0];
                }
            } else {
                switch (position) {
                    case "t":
                    case "lt":
                    case "rt":
                        return `${this.cMargin[0]}`;
                    case "b":
                    case "lb":
                    case "rb":
                        return `calc(100% - ${this.cHeight} - ${this.cMargin[2]})`;
                    case "l":
                    case "r":
                    case "auto":
                        return `calc((100% - ${this.cHeight})/2)`;
                    default:
                        return position;
                }
            }
            return `calc((100% - ${this.cHeight})/2)`;
        },
        // 窗体位置--left
        cLeft() {
            if (this.dLeftMoved) return this.dLeftMoved;
            let position = this.position;
            if (typeof position === "object") {
                if (position.length === 2) {
                    return position[1];
                }
            } else {
                switch (position) {
                    case "l":
                    case "lt":
                    case "lb":
                        return `${this.cMargin[3]}`;
                    case "r":
                    case "rt":
                    case "rb":
                        return `calc(100% - ${this.width} - ${this.cMargin[1]})`;
                    case "t":
                    case "b":
                    case "auto":
                        return `calc((100% - ${this.width})/2)`;
                }
            }
            return `calc((100% - ${this.width})/2)`;
        },
        // 窗体位置
        cPositionStyle() {
            let style;
            if (this.dIsMinimize) {
                style = {
                    width: "200px",
                    height: "40px",
                    bottom: "0px",
                    left: "0px",
                };
            } else if (this.dIsMaximize) {
                style = {
                    width: "100%",
                    height: "100%",
                    top: "0px",
                    left: "0px",
                };
            } else {
                style = {
                    width: this.width,
                    height: this.height,
                    top: this.cTop,
                    left: this.cLeft,
                };
            }
            return style;
        },
        // 标题样式
        cTitleStyle() {
            let btnCount = 0;
            if (this.minimize && !this.dIsMinimize) btnCount++;
            if (
                this.maximize &&
                ((!this.dIsMinimize && this.dIsMaximize) ||
                    (this.dIsMinimize && !this.dIsMaximize))
            )
                btnCount++;
            if (
                this.maximize &&
                ((!this.dIsMinimize && !this.dIsMaximize) ||
                    (this.dIsMinimize && this.dIsMaximize))
            )
                btnCount++;
            if (this.closeAble) btnCount++;
            return {
                cursor:
                    this.moveAble && !this.dIsMinimize && !this.dIsMaximize
                        ? "move"
                        : "default",
                "padding-left": this.icon || this.iconimg ? "40px" : "10px",
                "padding-right": btnCount * 40 + "px",
            };
        },
        // 遮罩样式
        cShadeStyle() {
            let style = {};
            style["pointer-events"] = this.shadeEvent ? "auto" : "none";
            if (typeof this.shade === "number") {
                style.background = "#000";
                style.opacity = this.shade;
            } else {
                style.background = this.shade;
            }
            return style;
        },
        // 是否显示最小化按钮
        cMinimize() {
            // 允许最小化，且不处于最小化状态
            return this.minimize && !this.dIsMinimize;
        },
        // 是否显示最大化/正常化按钮
        cNormalizate() {
            if (this.dIsMinimize) {
                // 如果处于最小化状态，允许恢复窗口状态
                return true;
            } else {
                // 如果处于显示状态，根据maximize决定是否允许最大化
                return this.maximize;
            }
        },
    },
    watch: {
        value(newVal) {
            this.dShow = newVal;
        },
    },
    methods: {
        // 窗口最小化
        Minimize() {
            this.dIsMinimize = true;
        },
        // 窗口最大化、窗口正常化
        Maximize() {
            if (this.dIsMinimize) {
                this.dIsMinimize = false;
            } else {
                this.dIsMaximize = !this.dIsMaximize;
            }
        },
        // 显示窗口
        Show() {
            this.dShow = true;
            this.$emit("input", true);
        },
        // 关闭窗口
        Close(after) {
            let _this = this;
            function close() {
                _this.dShow = false;
                _this.$emit("input", false);
                if (typeof after === "function") {
                    after();
                }
            }
            if (typeof this.beforeClose === "function") {
                this.beforeClose(close);
            } else {
                close();
            }
        },
        // 移动窗体
        TitleMouseDown(e) {
            if (!this.moveAble || this.dIsMinimize || this.dIsMaximize) return;
            let elWindow = e.currentTarget.parentNode;
            let disX = e.clientX - elWindow.offsetLeft;
            let disY = e.clientY - elWindow.offsetTop;
            document.onmousemove = (e) => {
                this.dLeftMoved = e.clientX - disX + "px";
                this.dTopMoved = e.clientY - disY + "px";
            };
            document.onmouseup = () => {
                document.onmousemove = null;
                document.onmouseup = null;
            };
        },
        // 遮罩点击事件
        ShadeClick() {
            if (this.shadeClose) {
                this.Close();
            }
        },
    },
    mounted() {
        this.dIsMaximize = this.fullscreen;
    },
};
</script>

<style lang='scss' scoped>
.popup-window-container {
    width: 100%;
    height: 100%;
    position: fixed;
    top: 0px;
    left: 0px;
    z-index: 1000;
    pointer-events: none;
}
.popup-window-shade {
    width: 100%;
    height: 100%;
    top: 0px;
    left: 0px;
}
.popup-window-window {
    background: #f8f8f8;
    position: absolute;
    pointer-events: auto;
    .popup-window-head {
        width: 100%;
        height: 40px;
        background: #f8f8f8;
        border-bottom: 1px solid #eee;
        // padding: 0px 120px 0px 40px;
        user-select: none;
        .icon {
            width: 40px;
            height: 40px;
            position: absolute;
            top: 0px;
            left: 0px;
            padding: 5px;
            .v-icon {
                width: 100%;
                height: 100%;
            }
        }
        .title {
            height: 100%;
            line-height: 40px;
            font-size: 16px;
            color: #333;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
        }
        .btns {
            height: 40px;
            position: absolute;
            top: 0px;
            right: 0px;
            .btn {
                width: 40px;
                height: 40px;
                cursor: pointer;
                &:hover {
                    background-color: #ccc;
                }
            }
        }
    }
    .popup-window-content {
        width: 100%;
        height: calc(100% - 40px);
        iframe {
            width: 100%;
            height: 100%;
            border: 0px;
        }
    }
}
</style>
