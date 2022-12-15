<template>
    <div class="absolute w-0 h-0 bg-darkGray bg-opacity-90 border-b-24 border-x-24 border-x-opacity-0" :style="'top:'+props.menuCoordinates.y+'px; left:'+(props.menuCoordinates.x-60)+'px;'" v-if="props.setMenu"></div>
    <div class="flex flex-col absolute p-1 bg-darkGray bg-opacity-90 rounded-md" :style="'top:'+(props.menuCoordinates.y)+'px; left:'+(props.menuCoordinates.x-60)+'px;'" v-if="props.setMenu">
        <h1 class="text-mWhite ml-px mr-2 my-1 text-sm font-light">Check the warning types to show:</h1>
        <div class="flex flex-row ml-2">
            <input ref="danger" type="checkbox" :checked="filter.danger">
            <label class="px-1 text-mWhite text-sm font-light">Danger alerts</label>
        </div>
        <div class="flex flex-row ml-2">
            <input ref="warning" type="checkbox" :checked="filter.warning">
            <label class="px-1 text-mWhite text-sm font-light">Warning alerts</label>
        </div>
        <div class="flex flex-row ml-2">
            <input ref="info" type="checkbox" :checked="filter.info">
            <label class="px-1 text-mWhite text-sm font-light">Information alerts</label>
        </div>
        <div class="flex flex-row justify-end my-1">
            <button class=" mx-px px-1 py-px text-sm font-light bg-buttonDisabled rounded-md hover:bg-buttonGray border border-buttonDisabled" @click="emit('toggle-menu', false)">Close</button>
            <button class=" mx-px px-1 py-px text-sm font-light bg-buttonDisabled rounded-md hover:bg-buttonGray border border-buttonDisabled" @click="applyFilters">Apply</button>
        </div>
    </div>
</template>

<script setup>
import { ref, reactive, defineProps, defineEmits } from 'vue';

const props = defineProps({
    // filter: Object,
    menuCoordinates: Object,
    setMenu: Boolean,
});

const filter = reactive({ danger: true, warning: true, info: true });
const danger = ref();
const warning = ref();
const info = ref();

const applyFilters = () => {
    filter.danger = danger.value.checked;
    filter.warning = warning.value.checked;
    filter.info = info.value.checked;
    emit('apply-filters', filter);
    emit('toggle-menu', false);
}

const emit = defineEmits(['apply-filters', 'toggle-menu']);
</script>