<template>
  <div class="flex flex-row">
    <div class="basis-auto relative h-screen w-[65px]">
      <the-nav-bar class="absolute inset-y-0 left-0 w-[65px]"></the-nav-bar>
    </div>
      <router-view class="basis-auto h-screen w-screen"></router-view>
  </div>
</template>

<script setup>
import { ref, provide } from 'vue';

import TheNavBar from './components/nav/TheNavBar.vue';

// const robots = ref(require('./assets/robotData.json'));
// const routes = ref(require('./assets/routeData.json'));

/*
if you have the memory available it may not be a bad idea performance-wise. Using node.js's require() on a json file loads the data into memory really fast.

I ran two tests to see what the performance looked like on printing out an attribute from each feature from a 81MB geojson file.

In the 1st test, I read the entire geojson file into memory using var data = require('./geo.json'). That took 3330 milliseconds and then printing out an attribute from each feature took 804 milliseconds for a grand total of 4134 milliseconds. However, it appeared that node.js was using 411MB of memory.
*/

const historicRoutes = ref(require('./assets/historicRouteData.json'));

// provide('robots', robots);
// provide('routes', routes);
provide('historicRoutes', historicRoutes);
</script>

<style>
body {
  background-image: url('./assets/background.png');
}
</style>
