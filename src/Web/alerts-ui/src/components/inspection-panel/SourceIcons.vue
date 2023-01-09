<template>
    <div v-for="source in sources" :key="source.id" class="absolute" :style="'top:'+source.coordinates.y+'px; left:'+source.coordinates.x+'px;'">
        <div v-if="source.type == 'camera'">
            <div class="absolute top-0" :style="'width: '+sourceSize+'px; height:'+sourceSize+'px; transform: rotate('+source.direction+'deg); transform-origin: top left;'">
                <div ref="triangle" class="absolute" :style="'left: '+getLeft()+'px; top: '+getTop(source.id)+'px; border-top: '+source.range/2+'px solid transparent; border-right: '+source.range+'px solid rgba(255,0,0,.75); border-bottom: '+source.range/2+'px solid transparent;'"></div>
                <div class="absolute top-0 rounded-full border border-mWhite bg-navBar z-10" :style="'width: '+sourceSize+'px; height:'+sourceSize+'px;'"></div>
            </div>
        </div>
        <div v-else-if="source.type == 'sensor'">
            <div v-if="source.sensorType == 'temperature'">
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width=".5" stroke="currentColor" class="w-6 h-6 stroke-navBar fill-red opacity-90">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M15.362 5.214A8.252 8.252 0 0112 21 8.25 8.25 0 016.038 7.048 8.287 8.287 0 009 9.6a8.983 8.983 0 013.361-6.867 8.21 8.21 0 003 2.48z" />
                    <path stroke-linecap="round" stroke-linejoin="round" d="M12 18a3.75 3.75 0 00.495-7.467 5.99 5.99 0 00-1.925 3.546 5.974 5.974 0 01-2.133-1A3.75 3.75 0 0012 18z" />
                </svg>

            </div>
            <div v-else>
                <div class="absolute top-0 rounded-full border border-red" :style="'width: '+sourceSize+'px; height:'+sourceSize+'px;'"></div>
                <div class="absolute text-red font-sans" :style="'left: '+sourceSize/5.5+'px; top: '+(-sourceSize/4)+'px; font-size: '+sourceSize+'px;'">{{ source.sensorType[0].toUpperCase() }}</div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, inject } from 'vue';

const sources = inject('sources');

const sourceSize = ref(15);
console.log(sources);

const triangle = ref();

const getLeft = () => {
    // let pickedSource = {};
    // for(const source of sources.value) {
    //     if(source.id == id) {
    //         pickedSource = source;
    //     }
    // }
    const original_x_component = sourceSize.value/2;
    // const original_y_component = -pickedSource.range/2+sourceSize.value/2;
    // const vector_module = Math.sqrt((original_x_component**2)+(original_y_component**2));
    // const vector_angle = Math.atan(original_y_component/original_x_component);
    // const left = vector_module*Math.cos(vector_angle-pickedSource.direction*Math.PI/180);
    // const angle = pickedSource.direction*Math.PI/180;
    // const left = original_x_component*Math.cos(-angle)+original_y_component*Math.sin(-angle);
    // console.log('Angle for '+pickedSource.name+': '+pickedSource.direction+'°='+angle+'rads');
    // console.log('Left for '+pickedSource.name+': '+left);
    return original_x_component; //+pickedSource.range/2*Math.sin(angle);
}

const getTop = (id) => {
    let pickedSource = {};
    for(const source of sources.value) {
        if(source.id == id) {
            pickedSource = source;
        }
    }
    // const original_x_component = -sourceSize.value/2;
    const original_y_component = -pickedSource.range/2+sourceSize.value/2;
    // const vector_module = Math.sqrt((original_x_component**2)+(original_y_component**2));
    // const vector_angle = Math.atan(original_y_component/original_x_component);
    // const top = vector_module*Math.sin(vector_angle-pickedSource.direction*Math.PI/180);
    // const angle = pickedSource.direction*Math.PI/180;
    // const top = original_x_component*Math.sin(-angle)+original_y_component*Math.cos(-angle);
    // console.log('Top for '+pickedSource.name+': '+top);
    return original_y_component; //+pickedSource.range/2*Math.sin(angle);;
}
</script>

<style scoped>
</style>