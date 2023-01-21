import '@babel/polyfill'
import 'mutationobserver-shim'
import Vue from 'vue'
import './plugins/bootstrap-vue'
import router from './router'
import axios from 'axios'
import Home from "@/pages/Home";
import { BootstrapVue, IconsPlugin } from 'bootstrap-vue'
import moment from 'moment';

/* import the fontawesome core */
import { library } from '@fortawesome/fontawesome-svg-core'

/* import specific icons */
import { faDisplay,faPenToSquare,faBookOpen,faBook,faUserPen } from '@fortawesome/free-solid-svg-icons'
import {faCircleXmark} from "@fortawesome/free-regular-svg-icons";

/* import font awesome icon component */
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

library.add(faDisplay,faPenToSquare,faBookOpen,faBook,faUserPen,faCircleXmark)
Vue.component('font-awesome-icon', FontAwesomeIcon)
Vue.config.productionTip = false

Vue.filter('formatDate', function(value) {
  if (value) {
    return moment(String(value)).format('DD.MM.YYYY')
  }
});

Vue.use(BootstrapVue)
Vue.use(IconsPlugin)
Vue.config.productionTip = false

axios.defaults.withCredentials = true

axios.interceptors.response.use(undefined, function (error) {
  if (error) {
    const originalRequest = error.config;
    if (error.response.status === 401 && !originalRequest._retry) {
  
        originalRequest._retry = true;
        return router.push('/login')
    }
  }
})

new Vue({
  el: '#app',
  router,
  render: h => h(Home)
}).$mount('#app')
