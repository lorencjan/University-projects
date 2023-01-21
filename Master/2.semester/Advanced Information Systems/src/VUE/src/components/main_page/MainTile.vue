<template>
  <b-col cols="2">
    <b-row>
      <b-img class="tile-img" :src="img" alt="Book cover" v-if="img !== null && img !== undefined"></b-img>
      <font-awesome-icon icon="fa-solid fa-book" size="9x" class="my-2 img-cover" v-else />

    </b-row>
    <b-row class="text-left">
      <b-badge pill variant="warning">{{ type }}</b-badge>
    </b-row>
    <b-row class="text-left mt-2">
      <router-link :to="{path: root + id}">
        <h5 class="text-left">
          {{ name }}
        </h5>
      </router-link>
    </b-row>
    <b-row class="text-left">
      <small v-for="author in authors">
        <router-link :to="{path: '/authors/'+author.id}" class="author-link">{{author.name}} {{author.surname}}</router-link>&nbsp;
      </small>
    </b-row>
    <b-row class="text-left" v-if="genres !== undefined">
      <small v-for="genre in genres">
        <router-link :to="{path: '/genre/'+genre.id}" class="author-link">{{genre.name}}</router-link>&nbsp;
      </small>
    </b-row>
    <b-row class="text-left" v-if="fields !== undefined">
      <small v-for="field in fields">
        <router-link :to="{path: '/field/'+field.id}" class="author-link">{{field.name}}</router-link>&nbsp;
      </small>
    </b-row>
  </b-col>
</template>

<script>

export default {
name: 'MainTile',
  props: {
    type: String,
    name: String,
    authors: {},
    genres: {},
    img: [],
    fields: {},
    id: Number,
    root: String,
  },
  methods: {
    getAuthors(data){
      let dataParsed = JSON.parse(JSON.stringify(data));
      let string = "";
      dataParsed.forEach((dato) => string += "&" + dato.surname + " " + dato.name);
      string = string.replace('&','')
      string = string.replaceAll("&",", ")
      return string;
    },
    getNames(data){
      let dataParsed = JSON.parse(JSON.stringify(data));
      let string = "";
      dataParsed.forEach((dato) => string += "&" + dato.name);
      string = string.replace('&','')
      string = string.replaceAll("&",", ")
      return string;
    }
  },
  computed: {
    authorsToPrint() {
      return this.getAuthors(this.authors);
    },
    fieldsToPrint() {
      return this.getNames(this.fields);
    },
    genresToPrint() {
      return this.getNames(this.genres);
    },

    imageSrc: function (){
      const base64String = btoa(String.fromCharCode(...new Uint8Array(this.img)));
      return "data:image/png;base64," + base64String;
    }
  }
}
</script>

<style scoped>
.author-link{
  color: inherit;
}
.author-link:hover{
  color: #333333;
}
.img-cover{
  color:#24433e;
}
</style>