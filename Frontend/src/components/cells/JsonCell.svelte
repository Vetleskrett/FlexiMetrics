<script lang="ts">
	export let value: string;

	const json = JSON.parse(value);
	const text = JSON.stringify(json, null, 2);
</script>

<div class="rounded-md bg-[#F2F2F2] p-2">
	<p style="white-space: pre-wrap">{text}</p>
</div>
