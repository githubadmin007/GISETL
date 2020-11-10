<template>
    <svg>
        <polyline :points="cPoints" style="fill:none;stroke:black;stroke-width:3" />
        <polyline :points="arrowsPoints" style="fill:none;stroke:black;stroke-width:3" />
    </svg>
</template>

<script>
export default {
    data() {
        return {};
    },
    props: {
        relation: {
            type: Object,
            default: () => {},
        },
    },
    computed: {
        // 前一节点中心点坐标
        cForntCenter() {
            return getCenter(this.relation.frontStep);
        },
        // 后一节点中心点坐标
        cAfterCenter() {
            return getCenter(this.relation.afterStep);
        },
        // 路径
        cPoints() {
            let strFront = this.cForntCenter.join(",");
            let strAfter = this.cAfterCenter.join(",");
            return `${strFront} ${strAfter}`;
        },
        arrowsPoints() {
            let strinser = getarrow(
                this.relation.frontStep,
                this.relation.afterStep
            );
            let insepoint = strinser.join(",");
            return `${insepoint}`;
        },
    },
};
// 获取步骤节点的中心坐标
function getCenter(step) {
    if (step) {
        let centerX = step.X + step.WIDTH / 2;
        let centerY = step.Y + step.HEIGHT / 2;
        return [centerX, centerY];
    }
    return [0, 0];
}

//绘制箭头
//直线上两点间距离确定一个点 再利用对称确定另一个点
function getarrow(frontStep, afterStep) {
    if (frontStep && afterStep) {
        //默认与侧边相交
        let arrowx = afterStep.X;
        let frontStepcen = getCenter(frontStep);
        let afterStepcen = getCenter(afterStep);
        let x1 = frontStepcen[0];
        let y1 = frontStepcen[1];
        let x2 = afterStepcen[0];
        let y2 = afterStepcen[1];
        let arrowy = ((y2 - y1) * (arrowx - x1)) / (x2 - x1) + y1;
        if (
            arrowy > y2 - afterStep.HEIGHT / 2 &&
            arrowy < y2 + afterStep.HEIGHT / 2
        ) {
            if (x1 < x2) {
                let k = (y2 - y1) / (x2 - x1);
                let b = y2 - k * x2;
                let k1 = (k - 1) / (1 + k);
                let b1 = arrowy - k1 * arrowx;
                let z1 = Math.sqrt(1 + 1 / (k1 * k1));
                let yy1 = 15 / z1 + arrowy;
                let xx1 = (yy1 - b1) / k1;
                let k2 = (1 + k) / (1 - k);
                let b2 = arrowy - k2 * arrowx;
                let xx2 = (k * xx1 + 2 * b - b2 - yy1) / (k2 - k);
                let yy2 = k2 * xx2 + b2;
                return [xx2, yy2, arrowx, arrowy, xx1, yy1]; //与左侧边相交
            } else if (x1 > x2) {
                let arrowx = afterStep.X + afterStep.WIDTH;
                let arrowy = ((y2 - y1) * (arrowx - x1)) / (x2 - x1) + y1;

                let k = (y2 - y1) / (x2 - x1);
                let b = y2 - k * x2;
                let k1 = (k - 1) / (1 + k);
                let b1 = arrowy - k1 * arrowx;
                let k2 = (1 + k) / (1 - k);
                let b2 = arrowy - k2 * arrowx;
                let z2 = Math.sqrt(1 + 1 / (k2 * k2));
                let yy2 = 15 / z2 + arrowy;
                let xx2 = (yy2 - b2) / k2;
                let xx1 = (k * xx2 + 2 * b - b1 - yy2) / (k1 - k);
                let yy1 = k1 * xx1 + b1;
                return [xx2, yy2, arrowx, arrowy, xx1, yy1];
            }
        } else {
            //默认与上边相交
            arrowy = afterStep.Y;
            arrowx = ((arrowy - y1) * (x2 - x1)) / (y2 - y1) + x1;
            if (y1 < y2) {
                let k = (y2 - y1) / (x2 - x1);
                let b = y2 - k * x2;
                let k2 = (1 + k) / (1 - k);
                let b2 = arrowy - k2 * arrowx;
                let k1 = (k - 1) / (1 + k);
                let b1 = arrowy - k1 * arrowx;

                let z2 = Math.sqrt(1 + k2 * k2);
                let xx2 = 15 / z2 + arrowx;
                let yy2 = k2 * xx2 + b2;

                let xx1 = (k * xx2 + 2 * b - b1 - yy2) / (k1 - k);
                let yy1 = k1 * xx1 + b1;

                return [xx2, yy2, arrowx, arrowy, xx1, yy1];
            } else if (y1 > y2) {
                arrowy = afterStep.Y + afterStep.HEIGHT;
                arrowx = ((arrowy - y1) * (x2 - x1)) / (y2 - y1) + x1;
                let k = (y2 - y1) / (x2 - x1);
                let b = y2 - k * x2;
                let k2 = (1 + k) / (1 - k);
                let b2 = arrowy - k2 * arrowx;
                let k1 = (k - 1) / (1 + k);
                let b1 = arrowy - k1 * arrowx;

                let z1 = Math.sqrt(1 + k1 * k1);
                let xx1 = 15 / z1 + arrowx;
                let yy1 = k1 * xx1 + b1;

                let xx2 = (k * xx1 + 2 * b - b2 - yy1) / (k2 - k);
                let yy2 = k2 * xx2 + b2;

                return [xx2, yy2, arrowx, arrowy, xx1, yy1];
            }
        }
    }
    return [0, 0, 0, 0, 0, 0];
}
</script>
