<script lang="ts">
	import 'src/app.css';
	import { userRole } from 'src/store';

	// This role system is only meant for development
	function setTeacher(){
		userRole.set(0)
	}

	function setStudent(){
		userRole.set(1)
	}

</script>

<div class="fixed z-50 flex h-16 w-full flex-row bg-[#1D6F8B] text-white">
	<a
		href="/courses"
		class="flex h-full w-28 items-center justify-center text-center hover:bg-black hover:bg-opacity-10"
	>
		<h2 class="text-lg font-semibold">Courses</h2>
	</a>
	<button class="flex h-full w-28 items-center justify-center text-center hover:bg-black hover:bg-opacity-10 text-green-300" on:click={setTeacher}>Set Teacher</button>
	<button class="flex h-full w-28 items-center justify-center text-center hover:bg-black hover:bg-opacity-10 text-red-400" on:click={setStudent}>Set Student</button>
</div>
<div class="py-16">
	<slot />
</div>
