<template>
    <div class="bg-gradient-to-br from-bgFrom to-bgTo pl-10 flex flex-col">
        <div class="pb-5 pt-4">
            <h1 class="font-sans text-5xl font-medium text-darkGray">Inspection Panel</h1>
        </div>
        <div class="relative flex flex-row w-full h-full">
            <inspection-table class="basis-1/2 h-[53rem] pr-3" :setMenu="setMenu" :filters="filters" @select-alert="selectAlert" @select-input="selectInput" @send-coordinates="setMenuCoordinates"></inspection-table>
            <div class="flex flex-col overflow-hidden basis-1/2">
                <map-image class="h-full w-full overflow-hidden self-center" :size="imageSize" @resize="resizeImage" @show-menu="toggleCreateMenu"></map-image>
                <div class="self-center pt-4">
                    <img v-if="imageSelected" src="../assets/videoSearch.png" alt="" class="h-[27rem] pr-3">
                    <div v-else-if="textSelected" class="relative flex flex-col border-2 rounded-md p-1.5 m-3 w-[40rem] h-[27rem] justify-center bg-mWhite overflow-auto">
                        <pre class="absolute top-0 left-0 font-sans p-1 text-2xl font-light text-darkGray">
                            <!-- Lorem ipsum dolor sit amet consectetur adipisicing elit. Veritatis, quas beatae neque error quasi dolores minima iste tempora officiis harum nisi placeat. Sit laboriosam perferendis iure quisquam nobis voluptates libero!
                            Dolor eos, eum consectetur, odio quas eius officiis modi illo incidunt ex amet! Impedit, autem accusantium at quam voluptatum iste est deserunt magni. Voluptates enim ut explicabo nesciunt optio cumque.
                            Labore velit ipsum dolores debitis deleniti eveniet. Distinctio pariatur nulla, harum excepturi ipsa iste consectetur error et aspernatur reprehenderit beatae atque quo necessitatibus repudiandae vitae velit odit ad architecto! Possimus?
                            Itaque dolor reiciendis, voluptatum iusto repellendus repudiandae nihil beatae. Nam itaque molestiae excepturi quam cumque dolores libero similique iusto nemo! Esse reiciendis exercitationem rem temporibus et non similique nostrum quaerat. -->
                            {{ textSource }} <br>
                            &emsp; – Position: {{ getPosition(selectedAlert.source) }} <br>
                            &emsp; - Day/Time: {{ selectedAlert.date }} <br>
                            &emsp; - Detection Confidence: {{ selectedAlert.confidence }}%
                        </pre>
                    </div>
                    <div v-else-if="hasTwoInputs" class="flex flex-col border-2 border-dashed rounded-md p-1.5 m-3 w-[40rem] h-[27rem] justify-center">
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width=".5" stroke="currentColor" class="w-20 h-20 self-center">
                            <path stroke-linecap="round" stroke-linejoin="round" d="M3.75 9.776c.112-.017.227-.026.344-.026h15.812c.117 0 .232.009.344.026m-16.5 0a2.25 2.25 0 00-1.883 2.542l.857 6a2.25 2.25 0 002.227 1.932H19.05a2.25 2.25 0 002.227-1.932l.857-6a2.25 2.25 0 00-1.883-2.542m-16.5 0V6A2.25 2.25 0 016 3.75h3.879a1.5 1.5 0 011.06.44l2.122 2.12a1.5 1.5 0 001.06.44H18A2.25 2.25 0 0120.25 9v.776" />
                        </svg>
                        <center><p class="font-sans text-2xl font-light text-darkGray self-center">Chosen alert has two inputs...<br>Click again some type of input to display it!</p></center>
                    </div>
                    <div v-else class="flex flex-col border-2 border-dashed rounded-md p-1.5 m-3 w-[40rem] h-[27rem] justify-center">
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width=".5" stroke="currentColor" class="w-20 h-20 self-center">
                            <path stroke-linecap="round" stroke-linejoin="round" d="M3.75 9.776c.112-.017.227-.026.344-.026h15.812c.117 0 .232.009.344.026m-16.5 0a2.25 2.25 0 00-1.883 2.542l.857 6a2.25 2.25 0 002.227 1.932H19.05a2.25 2.25 0 002.227-1.932l.857-6a2.25 2.25 0 00-1.883-2.542m-16.5 0V6A2.25 2.25 0 016 3.75h3.879a1.5 1.5 0 011.06.44l2.122 2.12a1.5 1.5 0 001.06.44H18A2.25 2.25 0 0120.25 9v.776" />
                        </svg>
                        <center><p class="font-sans text-2xl font-light text-darkGray self-center">No alert chosen yet...<br>Choose an alert from the table to display it!</p></center>
                    </div>
                </div>
            </div>
        </div>
        <warnings-menu :menuCoordinates="menuCoordinates" :setMenu="setMenu" @toggle-menu="toggleMenu" @apply-filters="applyFilters"></warnings-menu>
    </div>
</template>

<script setup>
import InspectionTable from '../components/inspection-panel/InspectionTable.vue';
import MapImage from '../components/image/MapImage.vue'
import WarningsMenu from '../components/inspection-panel/WarningsMenu.vue';

import { ref, reactive, provide, watch, onBeforeMount } from 'vue';

// const alerts = ref([]);

// onMounted(async () => {
//     const request = await fetch('http://localhost:3001/alertshandler/alerts/');
//     if (request.ok) {
//         alerts.value = await request.json();
//     }
//     console.log(alerts.value);
// });

// provide('alerts', alerts);

const sources = ref();
const alerts = ref();

onBeforeMount(async () => {
    sources.value = await loadSources();
    alerts.value = await loadAlerts();
    console.log(sources.value);
    console.log(alerts.value);
    console.log(await loadSwaggerAlerts());
});

const loadAlerts = async () => {
    let fetchedAlerts;
    const requestAlerts = await fetch('http://localhost:3001/alertshandler/alerts');
    if (requestAlerts.ok) {
        fetchedAlerts = await requestAlerts.json();
    }
    return fetchedAlerts;
}

const loadSources = async () => {
    let fetchedSources;
    const requestSources = await fetch('http://localhost:3001/sources/');
    if (requestSources.ok) {
        fetchedSources = await requestSources.json();
    }
    return fetchedSources;
}

const loadSwaggerAlerts = async () => {
    let fetchedSwaggerAlerts;
    const requestSwaggerAlerts = await fetch('http://localhost:64118/Alerts');
    if (requestSwaggerAlerts.ok) {
        fetchedSwaggerAlerts = await requestSwaggerAlerts.json();
    }
    return fetchedSwaggerAlerts;
}

provide('sources', sources);
provide('alerts', alerts);

const getPosition = (id) => {
    let pickedSource = {};
    for(const source of sources.value) {
        if(source.id == id) {
            pickedSource = source;
        }
    }
    const position = transformCoordinates(pickedSource.coordinates);
    return '('+position.y+' '+position.x+')';
}

const coordinateTopLeft = '40°25′30″N 3°41′22″O';
const coordinateBottomRight = '40°14′30″N 3°57′22″O';

// const getCoordinateOperation = (term1, term2, operation) => {
//     let difference = "";
//     const n_s1 = term1.split('″')[1].split(" ")[0];
//     const e_w1 = term1.split('″')[-1];
//     const n_s2 = term2.split('″')[1].split(" ")[0];
//     const e_w2 = term2.split('″')[-1];
//     const d_ns_1 = term1.split('°')[0].match(/(\d+)/);
//     const m_ns_1 = term1.split('°')[1].split('′')[0].match(/(\d+)/);
//     const s_ns_1 = term1.split('°')[1].split('′')[1].split('″')[0].match(/(\d+)/);
//     const d_ns_2 = term2.split('°')[0].match(/(\d+)/);
//     const m_ns_2 = term2.split('°')[1].split('′')[0].match(/(\d+)/);
//     const s_ns_2 = term2.split('°')[1].split('′')[1].split('″')[0].match(/(\d+)/);
//     const d_ew_1 = term1.split('°')[1].split(' ')[-1].match(/(\d+)/);
//     const m_ew_1 = term1.split('′')[1].split('°')[-1].match(/(\d+)/);
//     const s_ew_1 = term1.split('″')[1].split('′')[-1].match(/(\d+)/);
//     const d_ew_2 = term2.split('°')[1].split(' ')[-1].match(/(\d+)/);
//     const m_ew_2 = term2.split('′')[1].split('°')[-1].match(/(\d+)/);
//     const s_ew_2 = term2.split('″')[1].split('′')[-1].match(/(\d+)/);
//     if(n_s1 == n_s2 && operation == '-') {
//         let dif_d_ns = d_ns_1 - d_ns_2;
//         let dif_m_ns = (m_ns_1-m_ns_2)%60;

//         dif_d_ns += Math.floor((m_ns_1-m_ns_2)/60);

//     }
// }

const transformCoordinates = (coordinates) => {
    console.log(coordinates);
    return {
        x: coordinateTopLeft.split(' ')[1],
        y: coordinateBottomRight.split(' ')[0]
    };
}

// const alerts = inject('historicRoutes');

const selectedAlert = ref({});
const hasTwoInputs = ref(false);
const textSelected = ref(false);
const imageSelected = ref(false);
const textSource = ref('');
const imageSource = ref('');

const selectAlert = (id) => {
    selectedAlert.value = alerts.value.find((alert) => alert.id === id);
}

watch(selectedAlert, () => {
    if(selectedAlert.value.information.text != undefined && selectedAlert.value.information.picture != undefined) {
        hasTwoInputs.value = true;
        selectInput({ input: 'both' });
    } else if (selectedAlert.value.information.text != undefined) {
        hasTwoInputs.value = false;
        selectInput({ input: 'text', content: selectedAlert.value.information.text });
    } else if (selectedAlert.value.information.picture != undefined) {
        hasTwoInputs.value = false;
        selectInput({ input: 'image', content: selectedAlert.value.information.picture });
        textSelected.value = false;
    }
});

const selectInput = ($event) => {
    if($event.input == 'text') {
        textSelected.value = true;
        imageSelected.value = false;
        textSource.value = $event.content
    } else if($event.input == 'image') { 
        imageSelected.value = true;
        textSelected.value = false;
        imageSource.value = $event.content;
        console.log(imageSource.value);
    } else {
        textSelected.value = false;
        imageSelected.value = false;
    }
}

provide('selectedAlert', selectedAlert);

const imageSize = reactive({ width: 600, height: 343 });

const resizeImage = (size) => {
    imageSize.width = size.x;
    imageSize.height = size.y;
}

const menuCoordinates = reactive({});
const setMenu = ref(false);

const setMenuCoordinates = (coordinates, menu) => {
    menuCoordinates.x = coordinates.x;
    menuCoordinates.y = coordinates.y;
    toggleMenu(menu);
}

const toggleMenu = (value) => {
    setMenu.value = value;
}

const filters = reactive({ danger: true, warning: true, info: true });

const applyFilters = (filter) => {
    filters.danger = filter.danger;
    filters.warning = filter.warning;
    filters.info = filter.info;
}

const createMenuIsVisible = ref(false);
const createMenuCoordinates = reactive({});

const toggleCreateMenu = ($event) => {
    createMenuCoordinates.x = $event.clientX;
    createMenuCoordinates.y = $event.clientY;
    createMenuIsVisible.value = !createMenuIsVisible.value;
}
</script>