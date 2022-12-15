<template>
    <div class="flex flex-col w-full h-full overflow-auto">
        <div class="overflow-auto self-center" id="table" @scroll="checkScroll">
            <table class="table-fixed w-full">
                <tr class="sticky top-0">
                    <th class="border border-t-darkGray border-darkGray bg-tableHeader text-xl font-semibold tracking-wide w-1/6">Date</th>
                    <th class="border border-t-darkGray border-darkGray bg-tableHeader text-xl font-semibold tracking-wide w-5/12">
                        <div class="flex flex-row justify-center">
                            <h1 class="text-xl font-semibold tracking-wide">Source</h1>
                            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" class="ml-2 w-4 h-4 self-center stroke-[#000000] fill-[#546A7A] active:fill-mWhite hover:fill-buttonDisabled cursor-pointer">
                                <path fill-rule="evenodd" d="M3.792 2.938A49.069 49.069 0 0112 2.25c2.797 0 5.54.236 8.209.688a1.857 1.857 0 011.541 1.836v1.044a3 3 0 01-.879 2.121l-6.182 6.182a1.5 1.5 0 00-.439 1.061v2.927a3 3 0 01-1.658 2.684l-1.757.878A.75.75 0 019.75 21v-5.818a1.5 1.5 0 00-.44-1.06L3.13 7.938a3 3 0 01-.879-2.121V4.774c0-.897.64-1.683 1.542-1.836z" clip-rule="evenodd" />
                            </svg>
                        </div>
                    </th>
                    <th class="border border-t-darkGray border-darkGray bg-tableHeader w-3/12">
                        <div class="flex flex-row justify-center">
                            <h1 class="text-xl font-semibold tracking-wide">Type</h1>
                            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" class="ml-2 w-4 h-4 self-center stroke-[#000000] fill-[#546A7A] active:fill-mWhite hover:fill-buttonDisabled cursor-pointer" @click.prevent="showFilter">
                                <path fill-rule="evenodd" d="M3.792 2.938A49.069 49.069 0 0112 2.25c2.797 0 5.54.236 8.209.688a1.857 1.857 0 011.541 1.836v1.044a3 3 0 01-.879 2.121l-6.182 6.182a1.5 1.5 0 00-.439 1.061v2.927a3 3 0 01-1.658 2.684l-1.757.878A.75.75 0 019.75 21v-5.818a1.5 1.5 0 00-.44-1.06L3.13 7.938a3 3 0 01-.879-2.121V4.774c0-.897.64-1.683 1.542-1.836z" clip-rule="evenodd" />
                            </svg>
                        </div>
                    </th>
                    <th class="border border-t-darkGray border-darkGray bg-tableHeader text-xl font-semibold tracking-wide w-1/6">Information</th>
                </tr>
                <tr v-for="i in max" :key="filteredAlerts[i-1].id" @click="emit('select-alert', filteredAlerts[i-1].id)">
                    <td class="cursor-pointer border border-darkGray text-center py-1" :class="filteredAlerts[i-1].id == selectedAlert.id ? 'bg-routeSelected' : 'bg-tableData'">{{ getYear(filteredAlerts[i-1].date) }}<br>{{ getHour(filteredAlerts[i-1].date) }}</td>
                    <td class="cursor-pointer border border-darkGray text-center py-1" :class="filteredAlerts[i-1].id == selectedAlert.id ? 'bg-routeSelected' : 'bg-tableData'">{{ filteredAlerts[i-1].source }}</td>
                    <td class="cursor-pointer border border-darkGray text-center py-1" :class="filteredAlerts[i-1].id == selectedAlert.id ? 'bg-routeSelected' : 'bg-tableData'">
                        <alert-warnings :alert="filteredAlerts[i-1]"></alert-warnings>
                    </td>
                    <td class="cursor-pointer border border-darkGray text-center py-1" :class="filteredAlerts[i-1].id == selectedAlert.id ? 'bg-routeSelected' : 'bg-tableData'">
                        <alert-information :alert="filteredAlerts[i-1]" @select="emit('select-input', $event)"></alert-information>
                    </td> 
                </tr>
            </table>
        </div>
        <div class="flex flex-row justify-end">
            <button class="mr-10 px-2 my-2 py-1 font-sans font-light text-sm rounded-md self-right justify-self-end active:bg-mWhite" :class="bottomReached && endReached == false ? 'bg-tableHeader' : 'bg-buttonDisabled'" :disabled="(!bottomReached || endReached)" @click="loadMore()">Load More</button>
            <button class="mr-10 px-2 my-2 py-1 font-sans font-light text-sm rounded-md self-right justify-self-end active:bg-mWhite" :class="endReached == false ? 'bg-tableHeader' : 'bg-buttonDisabled'" :disabled="endReached" @click="loadAll()">Load All</button>
            <button class="px-2 my-2 py-1 font-sans font-light text-sm rounded-md self-right justify-self-end active:bg-mWhite bg-tableHeader" @click="loadData()">Refresh Data</button>
        </div>
    </div>
</template>

<script setup>
import AlertWarnings from "./AlertWarnings.vue";
import AlertInformation from "./AlertInformation.vue";

import { ref, inject, defineProps, defineEmits, reactive, watch, onMounted } from 'vue';

const props = defineProps({
    setMenu: Boolean,
    filters: Object,
});

// const alertss = inject('historicRoutes');
const alerts = ref();
const sources = ref();
// const filteredAlerts = ref(alertss.value);
const filteredAlerts = ref();
const max = ref(0);

onMounted(() => {
    alerts.value = inject('historicRoutes');
    sources.value = inject('sources');
    filteredAlerts.value = alerts.value;
    console.log(alerts.value);
    console.log(sources.value);
    console.log(filteredAlerts.value);
});

watch(props.filters, () => {
    filteredAlerts.value = [];
    for(const alert of alerts.value) {
        if(alert.priority == 'high' && props.filters.danger && !filteredAlerts.value.includes(alert)) {
            filteredAlerts.value.push(alert);
        }
        if(alert.priority == 'medium' && props.filters.warning && !filteredAlerts.value.includes(alert)) {
            filteredAlerts.value.push(alert);
        }
        if(alert.priority == 'low' && props.filters.info && !filteredAlerts.value.includes(alert)) {
            filteredAlerts.value.push(alert);
        }
    }
    max.value = filteredAlerts.value.length > 50 ? 50 : filteredAlerts.value.length;
});

watch(filteredAlerts.value, () => {
    console.log('Max: '+max.value);
    max.value = filteredAlerts.value.length > 50 ? 50 : filteredAlerts.value.length;
})

const selectedAlert = inject('selectedAlert');

const bottomReached = ref(false);
const endReached = ref(false);

const checkScroll = (el) => {
    if((el.target.offsetHeight + el.target.scrollTop) >= el.target.scrollHeight) {
        bottomReached.value = true;
    } else {
        bottomReached.value = false;
    }
}

const loadMore = () => {
    const newMax = max.value+50;
    if(newMax < filteredAlerts.value.length) {
        max.value = newMax;
    } else {
        max.value = filteredAlerts.value.length;
        endReached.value = true;
    }
}

const loadAll = () => {
    max.value = filteredAlerts.value.length;
    endReached.value = true;
}

const loadData = async () => {

    const request = await fetch('http://localhost:3001/alertshandler/alerts');
    if (request.ok) {
        alerts.value = await request.json();
        filteredAlerts.value = alerts.value;
        console.log(filteredAlerts.value);
    }
    console.log(request);
}

const filterMenu = ref(false);
const menuCoordinates = reactive({ x: 0, y: 0 });

const showFilter = ($event) => {
    filterMenu.value = props.setMenu.value;
    filterMenu.value = !filterMenu.value;
    menuCoordinates.x = $event.clientX;
    menuCoordinates.y = $event.clientY;
    emit('send-coordinates', menuCoordinates, filterMenu.value);
}

const getYear = (time) => {
    const date = new Date(time);
    return date.getFullYear()+'/'+date.getMonth()+'/'+date.getDate();
}
const getHour = (date) => {
    const time = new Date(date);
    return time.getHours()+':'+time.getMinutes();
}

// const getSource = (id) => {
//     console.log(sources);
//     let pickedSource = {};
//     for(const source of sources.value) {
//         if(source.id == id) {
//             pickedSource = source;
//         }
//     }
//     console.log(pickedSource);
//     return source ? source.name : '';
// }

const emit = defineEmits(['select-alert', 'select-input', 'send-coordinates']);
</script>

<style scoped>
#table::-webkit-scrollbar {
    appearance: none;
}

#table::-webkit-scrollbar:vertical {
    width: 5px;
    background-color: #34495E;
    border-radius: 8px;
}

#table::-webkit-scrollbar:horizontal {
    height: 0px;
}

#table::-webkit-scrollbar-thumb {
    border-radius: 8px;
    background-color: rgba(243, 243, 243, .8);
}
</style>