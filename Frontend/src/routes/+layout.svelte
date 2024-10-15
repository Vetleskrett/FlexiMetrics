<script lang="ts">
	import 'src/app.css';
</script>

<div class="fixed flex h-16 w-full flex-row bg-[#1D6F8B] text-white">
	<a
		href="/courses"
		class="flex h-full w-28 items-center justify-center text-center hover:bg-black hover:bg-opacity-10"
	>
		<h2 class="text-lg font-semibold">Courses</h2>
	</a>
</div>
<div class="py-16">
	<slot />
</div>
