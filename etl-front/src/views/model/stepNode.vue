20200911stepnode.vue
<template>
<svg @mousedown="OnMouseDown" @click="OnClick" :x="step.X" :y="step.Y">
    <rect
        v-if="select"
        :width="step.WIDTH"
        :height="step.HEIGHT"
        fill="rgba(0,0,0,0)"
        stroke-width="1"
        stroke="rgb(255,0,0)"
    />

    <rect
        :width="step.WIDTH"
        :height="step.HEIGHT"
        fill="#FFF"
        stroke-width="1"
        stroke="#000"
        rx="10"
        ry="10"
    />

    <foreignObject :width="step.WIDTH" :height="step.HEIGHT" :y="namehight-20">
        <body xmlns="http://www.w3.org/1999/xhtml">
            <div
                class="s-content"
                style="user-select:none;color:red;text-align:center;word-wrap: break-word ;"
            >{{step.NAME}}</div>
        </body>
    </foreignObject>
</svg>
</template>

<script>
export default {
    data() {
        return {
            dnamelen: "",
            namehight: this.step.HEIGHT / 2 + 7,
        };
    },
    props: {
        step: {
            type: Object,
            default: () => {},
        },
        select: {
            type: Boolean,
            default: false,
        },
    },
    methods: {
        OnMouseDown(e) {
            this.$emit("mousedown", e);
        },
        OnClick(e) {
            this.$emit("click", e);
        },
    },
    computed: {
        namelen: {
            get() {
                return this.step.NAME.length || this.dnamelen;
            },
            set(value) {
                this.dnamelen = value;
            },
        },
    },
    watch: {
        namelen: {
            handler() {
                // todo: 待优化
                if (this.namelen < 3) {
                    this.step.WIDTH = 60;
                    this.step.HEIGHT = 50;
                } else if (this.namelen > 15) {
                    this.step.WIDTH = 15 * 20;
                    this.step.HEIGHT = 50;
                } else {
                    this.step.WIDTH = this.namelen * 20;
                    this.step.HEIGHT = 50;
                }
                // if (this.namelen < 3) {
                //     this.step.WIDTH = 60;
                //     this.step.HEIGHT = 50;
                // } else if (this.namelen > 15) {
                //     this.step.WIDTH = 15 * 20;
                //     var num = parseInt(this.namelen / 15);
                //     this.step.HEIGHT = 50 + 20 * (num + 1);
                // } else {
                //     this.step.WIDTH = this.namelen * 20;
                //     this.step.HEIGHT = 50;
                // }
            },
            immediate: true,
            deep: true,
        },
    },
};
</script>


