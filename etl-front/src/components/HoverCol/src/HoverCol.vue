<template>
    <v-col :cols="cols" :sm="sm" :md="md" :lg="lg">
        <v-hover v-slot:default="{ hover }">
            <v-card class="mx-auto" style="cursor:pointer;" :elevation="hover ? 12 : 2" outlined>
                <slot></slot>
            </v-card>
        </v-hover>
    </v-col>
</template>

<script>
export default {
    name: "VHoverCol",
    data() {
        return {};
    },
    props: {
        cols: String,
        sm: String,
        md: String,
        lg: String,
    },
};
</script>
