<template>
    <div ref="canvas" class="relative overflow-hidden overscroll-contain max-w-none" :style="'width:'+initialSize.width+'px; height:'+initialSize.height+'px;'">
        <div class="absolute bottom-1 right-1 z-50">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-7 h-7 p-1 stroke-darkGray bg-buttonDisabled bg-opacity-70 mb-px rounded-t-md hover:cursor-pointer" @click.prevent="zoom(10)">
                <path stroke-linecap="round" stroke-linejoin="round" d="M21 21l-5.197-5.197m0 0A7.5 7.5 0 105.196 5.196a7.5 7.5 0 0010.607 10.607zM10.5 7.5v6m3-3h-6" />
            </svg>
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-7 h-7 p-1 stroke-darkGray bg-buttonDisabled bg-opacity-70 rounded-b-md hover:cursor-pointer" @click.prevent="zoom(-10)">
                <path stroke-linecap="round" stroke-linejoin="round" d="M21 21l-5.197-5.197m0 0A7.5 7.5 0 105.196 5.196a7.5 7.5 0 0010.607 10.607zM13.5 10.5h-6" />
            </svg>
        </div>
        <div id="container" ref="container" class="relative h-full w-full overflow-scroll">
            <div id="image" ref="image" class="cursor-grab active:cursor-grabbing" :style="'transform: scale('+zoomValue+');'" @mousemove.prevent="imageScroll($event)" @mousedown="toggleClick(true)" @mouseup="toggleClick(false)"  @wheel.prevent="wheelEvents" @mouseleave="toggleClick(false)" @contextmenu.prevent="emit('show-menu', $event)">
                <source-icons></source-icons>
            </div>
        </div>
    </div>
</template>

<script setup>
/* Arreglar:
        Hace zoom respecto al centro, no donde tengas el ratón
*/
import SourceIcons from '../inspection-panel/SourceIcons.vue';

import { ref, reactive, defineProps, defineEmits, toRaw, onBeforeMount } from 'vue';

const props = defineProps({
    size: Object,
});

let initialSize = {};

onBeforeMount(() => {
    initialSize.width = toRaw(props.size.width);
    initialSize.height = toRaw(props.size.height);
})

const clicked = ref(false);
const container = ref();
const image = ref();
const canvas = ref();

const imageScroll = ($event) => {
    if(clicked.value) { 
        container.value.scrollLeft -= $event.movementX;
        container.value.scrollTop -= $event.movementY;
        // image.value.scrollLeft -= $event.movementX;
        // image.value.scrollTop -= $event.movementY;
        // console.log(image.value.scrollLeft, image.value.scrollTop);
    }
}

const toggleClick = (value) => {
    clicked.value = value;
}

const zoomValue = ref(1);

const zoom = (direction) => {
    if(zoomValue.value + direction*0.01 >= 1) 
        zoomValue.value += direction*0.01;
        // container.value.scrollLeft += direction*1600;
        // container.value.scrollTop += direction*900;
        // console.log(container.value.clientLeft, container.value.clientTop);
}

let deltaX = 0;
let deltaY = 0;
let startPoint = reactive({});
const wheelEvents = ($event) => {
    deltaX += $event.deltaX;
    deltaY += $event.deltaY;
    startPoint.x = $event.clientX;
    startPoint.y = $event.clientY;
    if(deltaX >= 16 || deltaX <= -16) {
        zoom(Math.round(deltaX/16));
        deltaX = 0;
        deltaY = 0;
    } else if(deltaY >= 9 || deltaY <= -9) {
        zoom(Math.round(deltaY/9));
        deltaX = 0;
        deltaY = 0;
    }
    // zoom($event.deltaY);
}

const emit = defineEmits('show-menu');
</script>

<style scoped>
#image {
    background-image: url('../../assets/map.jpg'); 
    height: 100%;
    width: 100%;
    background-position: center;
    background-repeat: no-repeat;
    background-size: cover;    
}
</style>