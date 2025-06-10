<script lang="ts">
	import 'src/app.css';
	import { Toaster } from '$lib/components/ui/sonner';
	import { House } from 'lucide-svelte';
</script>

<nav class="fixed z-50 flex h-16 w-full flex-row bg-[#1D6F8B] text-white">
	<a
		href="/"
		class="flex h-full w-28 items-center justify-center text-center hover:bg-black hover:bg-opacity-10"
	>
		<House size="32"/>
	</a>
	<a
		href="/teacher/courses"
		class="flex h-full w-28 items-center justify-center text-center hover:bg-black hover:bg-opacity-10"
	>
		<h2 class="text-lg font-semibold">Courses (teacher)</h2>
	</a>
	<a
		href="/student/courses"
		class="flex h-full w-28 items-center justify-center text-center hover:bg-black hover:bg-opacity-10"
	>
		<h2 class="text-lg font-semibold">Courses (student)</h2>
	</a>
</nav>
<div class="pt-16">
	<Toaster />
	<slot />
</div>
