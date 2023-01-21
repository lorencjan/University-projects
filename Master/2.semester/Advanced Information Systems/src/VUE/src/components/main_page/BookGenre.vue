<template>
  <b-dropdown-item v-on:click="redirect"> {{genre}}</b-dropdown-item>
</template>

<script>
export default {
  name: 'BookGenre',
  props: {
    genre: String,
    id: Number,
  },
  methods : {
    redirect() {
      this.$router.push({path: '/genre/' + this.id})
    }
  }
}
</script>
