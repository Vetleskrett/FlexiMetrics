<script lang="ts">
	export let value: Date;
</script>

<p>{new Date(value).toDateString()} {new Date(value).toLocaleTimeString()}</p>
