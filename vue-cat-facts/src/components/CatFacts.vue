<template>
  <div class="container mt-4">
    <h2 class="mb-3">Cat Facts</h2>

    <!-- Search Bar -->
    <input
      v-model="searchQuery"
      class="form-control mb-3"
      placeholder="Search cat facts..."
    />

    <!-- Table -->
    <table class="table table-striped">
      <thead>
        <tr>
          <th @click="sortBy('fact')">Fact</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="fact in filteredFacts" :key="fact.id">
          <td>{{ fact.fact }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
export default {
  data() {
    return {
      facts: [],
      searchQuery: "",
      sortColumn: "id",
      sortAscending: true,
    };
  },
  computed: {
    // Filter and sort the facts
    filteredFacts() {
      let filtered = this.facts;

      // Filter by search query
      if (this.searchQuery) {
        filtered = filtered.filter((fact) =>
          fact.fact.toLowerCase().includes(this.searchQuery.toLowerCase())
        );
      }

      // Sort data
      return filtered.sort((a, b) => {
        const valueA = a[this.sortColumn];
        const valueB = b[this.sortColumn];

        if (typeof valueA === "string") {
          return this.sortAscending
            ? valueA.localeCompare(valueB)
            : valueB.localeCompare(valueA);
        }

        return this.sortAscending ? valueA - valueB : valueB - valueA;
      });
    },
  },
  methods: {
    async fetchCatFacts() {
      try {
        const response = await fetch("https://localhost:7298/api/CatFact");
        if (!response.ok) throw new Error("API error");
        this.facts = await response.json();
      } catch (error) {
        console.error("Error fetching data:", error);
      }
    },
    sortBy(column) {
      if (this.sortColumn === column) {
        this.sortAscending = !this.sortAscending;
      } else {
        this.sortColumn = column;
        this.sortAscending = true;
      }
    },
  },
  mounted() {
    this.fetchCatFacts();
  },
};
</script>

<style>
th {
  cursor: pointer;
}
</style>
