<template>
    <div class="flex flex-row justify-center px-2">
        <div v-if="hasDanger" class="flex flex-row mx-4 py-1 px-2 bg-red bg-opacity-30 border border-darkRed border-opacity-50 rounded-md">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" class="w-5 h-5 mr-2 fill-darkRed">
                <path fill-rule="evenodd" d="M9.401 3.003c1.155-2 4.043-2 5.197 0l7.355 12.748c1.154 2-.29 4.5-2.599 4.5H4.645c-2.309 0-3.752-2.5-2.598-4.5L9.4 3.003zM12 8.25a.75.75 0 01.75.75v3.75a.75.75 0 01-1.5 0V9a.75.75 0 01.75-.75zm0 8.25a.75.75 0 100-1.5.75.75 0 000 1.5z" clip-rule="evenodd" />
            </svg>
            <div class="text-darkRed text-center text-sm">alert-danger</div>
        </div>
        <div v-if="hasWarnings" class="flex flex-row mx-4 py-1 px-2 bg-yellow bg-opacity-30 border border-darkYellow border-opacity-50 rounded-md">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" class="w-5 h-5 mr-2 fill-darkYellow">
                <path fill-rule="evenodd" d="M2.25 12c0-5.385 4.365-9.75 9.75-9.75s9.75 4.365 9.75 9.75-4.365 9.75-9.75 9.75S2.25 17.385 2.25 12zM12 8.25a.75.75 0 01.75.75v3.75a.75.75 0 01-1.5 0V9a.75.75 0 01.75-.75zm0 8.25a.75.75 0 100-1.5.75.75 0 000 1.5z" clip-rule="evenodd" />
            </svg>
            <div class="text-darkYellow text-center text-sm">alert-warning</div>
        </div>
        <div v-if="hasInfo" class="flex flex-row mx-4 py-1 px-2 bg-darkGray bg-opacity-30 border border-darkGray border-opacity-50 rounded-md">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" class="w-5 h-5 mr-2 fill-buttonGray">
                <path fill-rule="evenodd" d="M2.25 12c0-5.385 4.365-9.75 9.75-9.75s9.75 4.365 9.75 9.75-4.365 9.75-9.75 9.75S2.25 17.385 2.25 12zm8.706-1.442c1.146-.573 2.437.463 2.126 1.706l-.709 2.836.042-.02a.75.75 0 01.67 1.34l-.04.022c-1.147.573-2.438-.463-2.127-1.706l.71-2.836-.042.02a.75.75 0 11-.671-1.34l.041-.022zM12 9a.75.75 0 100-1.5.75.75 0 000 1.5z" clip-rule="evenodd" />
            </svg>
            <div class="text-buttonGray text-center text-sm">alert-info</div>
        </div>
    </div>
</template>

<script setup>
import { ref, defineProps, onMounted } from 'vue';

const props = defineProps({
    alert: Object,
});

const hasDanger = ref(false);
const hasWarnings = ref(false);
const hasInfo = ref(false);

onMounted(() => {
    if(props.alert.priority == 'high') {
        hasDanger.value = true;
    } else if(props.alert.priority == 'medium') {
        hasWarnings.value = true;
    } else if(props.alert.priority == 'low') {
        hasInfo.value = true;
    }
});
</script>