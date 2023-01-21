<template>
  <div id="Field">
    <NavbarFinal></NavbarFinal>
    <div>
      <h1 style="text-align:center">{{ this.field_name.name }}</h1>
    </div>
    <MainSection v-if="magazines.length" name="Magazines" :fullPage="'/magazines/?fields='+this.field_name.name" :data="magazines" root="/magazines/"></MainSection>
    <AuthorSection v-if="authors.length"
                   name="Authors"
                   :data="authors"
                   root="/authors/"
                   :fullPage="fullPage">

    </AuthorSection>
    <h3 style="text-align: center" v-if="! magazines.length && ! authors.length">No magazines and authors for this field.</h3>
  </div>
</template>

<script>

import MainSection from "@/components/main_page/MainSection";
import ApiConnect from "@/services/ApiConnect";
import AuthorSection from "@/components/genre_page/AuthorSection";
import NavbarFinal from "@/components/main_page/NavbarFinal";

export default {
  name: "Field",
  components: {
    AuthorSection,
    MainSection,
    NavbarFinal
  },

  data(){
    return {
      magazines: [],
      field_name: [],
      authors: [],
      fullPage : '/field_authors/' + this.$route.params.id
    }
  },

  methods: {
    getMagazines(name){
      let params = {params:{"fields": name}};
      ApiConnect.get('magazines/',params).then((response) =>
          {
            this.magazines = response.data;
            this.magazines.forEach(magazine => this.authors.push(magazine.authors[0]))
          }
      )},

    getName(){
      let id = this.$route.params.id
      if (typeof(this.$route.params.id) == 'undefined'){
        id = ''
      }
      ApiConnect.get('fields/'+id).then((response) =>
          {
            this.field_name = response.data;
            this.getMagazines(this.field_name.name);
          }
      )},


  },
  created() {
    this.getName();
  },

}

</script>

<style scoped>
@import "../assets/styles/main.css";
</style>