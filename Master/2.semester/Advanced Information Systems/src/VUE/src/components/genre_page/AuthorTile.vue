<template>
  <b-col cols="2" class="mx-2">
    <b-row class="text-center">
      <b-img class="b-skeleton-img" :src="img" alt="Author photo" v-if="img !== null && img !== undefined"></b-img>
      <font-awesome-icon icon="fa-solid fa-user-pen" size="10x" class="my-3 img-cover" v-else />
    </b-row>

    <b-row class="text-left mt-2">
      <router-link :to="{path: '/authors/' + id}">
        <h5 class="text-left">
          {{ name }} {{ surname }}
        </h5>
      </router-link>
    </b-row>
    <b-row class="text-left">
      <h6>{{ dateOfBirth | formatDate}}</h6>
    </b-row>
  </b-col>
</template>

<script>
export default {
  name: 'AuthorTile',
  props: {
    name: String,
    surname: String,
    img: String,
    id: Number,
    dateOfBirth: Date,
  },
  computed:{
    imageSrc: function (){
      const base64String = btoa(String.fromCharCode(...new Uint8Array(this.img)));
      return "data:image/png;base64," + base64String;
    }
  }
}
</script>

 <style scoped>
 h6{
   color: black;
 }
.img-cover{
  color:#24433e;
}
 </style>
