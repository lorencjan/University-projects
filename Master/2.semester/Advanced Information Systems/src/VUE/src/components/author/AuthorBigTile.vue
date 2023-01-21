<template>
  <div class="Author-big-tile">
    <b-row align-v="center">
      <b-col cols="3">
        <b-img :src="img" class="author-img" alt="Author Photo" v-if="img !== null && img !== undefined"></b-img>
        <font-awesome-icon icon="fa-solid fa-user-pen" size="10x" class="img-cover" v-else />
      </b-col>
      <b-col id="author-info" class="py-2 px-4 pt-4">
        <h1 class="text-left display-3 mb-0"> {{ name }} {{ surname }}</h1>
        <h2 class="text-left mt-2" v-if="birth">{{ birth | formatDate }} - <span v-if="death"> {{ death | formatDate}} </span></h2>
        <p class="text-justify">{{description}}</p>
      </b-col>

    </b-row>
  </div>
</template>

<script>
export default {
  name: "AuthorBigTile",
  props: {
    name: String,
    birth: Date,
    surname: String,
    description: String,
    death: Date,
    img: []
  },
  computed: {
    imageSrc: function (){
      const base64String = btoa(String.fromCharCode(...new Uint8Array(this.img)));
      return "data:image/png;base64," + base64String;
    }
  }

}
</script>

<style scoped>
#author-info{
  background-color: #24433e;
  color: #d0d3d3 !important;
}
h5, p{
  color: #d0d3d3 !important;
}
.display-3{
  color: #d0d3d3 !important;
  margin-bottom: -15px !important;
}
small{
  color: #cbcbcb !important;
}
.img-cover{
  color:#24433e;
}
</style>