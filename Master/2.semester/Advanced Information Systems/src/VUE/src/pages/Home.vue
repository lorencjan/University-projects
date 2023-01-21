<template>
  <div id="app">
    <div class="wrapper">
      <router-view :key="$route.path" ></router-view>
    </div>
    <MyFooter></MyFooter>
  </div>
</template>

<script>
import MyFooter from "@/components/main_page/MyFooter";
import NavbarFinal from "@/components/main_page/NavbarFinal";

export default {
  name: "Home",
  components:{
    MyFooter,
    NavbarFinal
  }
}
</script>

<style scoped>

</style>