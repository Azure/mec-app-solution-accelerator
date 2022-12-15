import { createRouter, createWebHistory } from 'vue-router';

import InspectionPanel from './pages/InspectionPanel.vue';

const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: '/inspectionPanel',
            component: InspectionPanel,
        },
    ],
    linkActiveClass: 'active',
});

export default router;