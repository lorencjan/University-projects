<template>
  <div id="books">
    <NavbarFinal></NavbarFinal>
    <b-container>
      <h1><span v-if="this.$route.query.fields !== undefined">{{ this.$route.query.fields }}</span>  Magazines:</h1>
      <b-row>
        <MainTile v-for="book in magazines"
                  :img="book.coverPhotoPath"
                  :genres = book.genres
                  :type = book.language
                  :name = book.name
                  :authors = book.authors
                  :fields = book.fields
                  :id = book.id
                  root = "/magazines/"
        >

        </MainTile>
      </b-row>
    </b-container>
  </div>
</template>

<script>
import MainTile from "@/components/main_page/MainTile";
import NavbarFinal from "@/components/main_page/NavbarFinal";
import ApiConnect from "@/services/ApiConnect";


export default {
  name: "Magazines",
  components: {
    MainTile,
    NavbarFinal
  },
  data(){
    return {
      magazines: [],
    }
  },
  methods: {
    getMagazines(){
      let params;
      if(this.$route.query.fields !== undefined) {
        params = {params: {"fields": this.$route.query.fields}}
      }else{
        params = {params: {}}
      }
      ApiConnect.get('magazines/', params).then((response) =>
          this.magazines = response.data,
      )},
  },
  created() {
    this.getMagazines();
  },
}
</script>

<style scoped>

</style>