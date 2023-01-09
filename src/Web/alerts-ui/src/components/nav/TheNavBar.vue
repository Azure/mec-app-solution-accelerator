<template>
  <div class="flex flex-col justify-between bg-navBar">
    <div>
      <img class="h-auto w-auto px-2 py-4" src="../../assets/microsoft_logo.png" alt="MicrosoftLogo"/>
      <hr class="h-2 mx-4 border-mWhite">
      <router-link to="/inspectionPanel">
        <stats-icon :class="statsClass ? 'bg-selected' : ''" @click="select('/inspectionPanel')"></stats-icon>
      </router-link>
    </div>
    <div>
      <hr class="h-2 mx-4 mb-20 border-mWhite">
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRoute } from 'vue-router';

import StatsIcon from '../icons/StatsIcon.vue';

const statsClass = ref(false);

function select(selectedButton) {
  statsClass.value = false;
  if(selectedButton == '/inspectionPanel') {
    statsClass.value = true;
  }
}

// Esto no funciona bien...
onMounted(function() {
  const route = useRoute();
  const path = computed(() => route.path);
  console.log(path.value);
  select(path.value);
});
</script>

<style>
a.active {
  background-color: #2A73D9;
}
</style>
