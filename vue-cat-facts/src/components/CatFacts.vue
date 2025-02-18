<template>
  <div>
    <h1>Cat Facts</h1>
    <ul>
      <li v-for="fact in catFacts" :key="fact.id">{{ fact.fact }}</li>
    </ul>
  </div>
</template>

<script>
export default {
  data() {
    return {
      catFacts: []
    };
  },
  mounted() {
    this.fetchCatFacts();
  },
  methods: {
    async fetchCatFacts() {
      try {
        var response = await fetch('https://localhost:7298/api/CatFact');
        if (!response.ok) {
          throw new Error('Failed to fetch cat facts');
        }
        var data = await response.json();
        console.log(data);
        this.catFacts = data;
      } catch (error) {
        console.error("Error fetching cat facts:", error);
      }
    }
  }
};
</script>

<style scoped>
</style>
