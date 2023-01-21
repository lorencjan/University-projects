<template>
  <div>
  <b-container class="bv-example-row">
    <b-row>
      <b-col><h1>{{ name }}</h1></b-col>
      <b-col>
        <router-link :to="fullPage">
          <h3>More ></h3>
        </router-link>
      </b-col>
    </b-row>

    <b-row v-if="data">
      <MainTile v-for="load in data.slice(0,6)" :key="load.id"
                :genres = "load.genres"
                :type = "load.language"
                :name = "load.name"
                :authors = "load.authors"
                :fields = "load.fields"
                :img="load.coverPhotoPath"
                :id = "load.id"
                :root = "root"
                >
      </MainTile>
    </b-row>
  </b-container>
  </div>
</template>


<script>
import MainTile from "@/components/main_page/MainTile";

export default {
  name: 'MainSection',
  components: {
    MainTile,
  },
  props: {
    name: String,
    fullPage: String,
    data: {},
    root: String,
  },

}

</script>
