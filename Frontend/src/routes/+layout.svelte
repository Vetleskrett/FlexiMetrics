<script lang="ts">
	import 'src/app.css';
</script>

<div class="fixed z-50 flex h-16 w-full flex-row bg-[#1D6F8B] text-white">
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
	<a
		href="/auth/signout"
		class="ml-auto flex h-full w-28 items-center justify-center text-center hover:bg-black hover:bg-opacity-10"
	>
		<h2 class="text-lg font-semibold">Sign Out</h2>
	</a>
</div>
<div class="py-16">
	<slot />
</div>
