<script lang="ts">
	export let value: Date;
</script>

<p>
	<span class="text-nowrap">{new Date(value).toDateString()}</span>
	<span>{new Date(value).toLocaleTimeString()}</span>
</p>
